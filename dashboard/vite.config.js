import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Forwards any /api/* request to the ASP.NET backend running locally.
      // Adjust the target if your backend listens on a different port.
      '/api': {
        target: 'https://localhost:7103',
        changeOrigin: true,
        secure: false, // allow the backend's local dev HTTPS certificate
      },
    },
  },
})
