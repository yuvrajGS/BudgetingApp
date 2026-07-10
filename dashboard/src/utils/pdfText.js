// pdf.js is only needed on the statement-import page, so it's dynamically
// imported here rather than pulled into the main bundle for every route.
async function loadPdfjs() {
  const pdfjsLib = await import("pdfjs-dist");
  const { default: workerUrl } = await import("pdfjs-dist/build/pdf.worker.min.mjs?url");
  pdfjsLib.GlobalWorkerOptions.workerSrc = workerUrl;
  return pdfjsLib;
}

// Reads a PDF file (from an <input type="file">) and returns its text
// content as an array of lines, in reading order, across all pages.
export async function extractPdfLines(file) {
  const pdfjsLib = await loadPdfjs();
  const buffer = await file.arrayBuffer();
  const pdf = await pdfjsLib.getDocument({ data: buffer }).promise;

  const lines = [];
  for (let pageNum = 1; pageNum <= pdf.numPages; pageNum += 1) {
    const page = await pdf.getPage(pageNum);
    const content = await page.getTextContent();

    // Group text items into lines using their vertical position — pdf.js
    // gives us individual words/fragments, not lines, so nearby "y"
    // coordinates are treated as the same line.
    const rows = new Map();
    for (const item of content.items) {
      if (!item.str || !item.str.trim()) continue;
      const y = Math.round(item.transform[5]);
      const row = rows.get(y) ?? [];
      row.push({ x: item.transform[4], str: item.str });
      rows.set(y, row);
    }

    const sortedY = [...rows.keys()].sort((a, b) => b - a);
    for (const y of sortedY) {
      const line = rows
        .get(y)
        .sort((a, b) => a.x - b.x)
        .map((r) => r.str)
        .join(" ")
        .replace(/\s+/g, " ")
        .trim();
      if (line) lines.push(line);
    }
  }

  return lines;
}
