import { useState } from "react";
import { patchTransaction } from '../api/transactions';
import CircularProgress from '@mui/material/CircularProgress';

export function UpdateTransactionCategoryBox({ categoryMap, categoryId, transactionId }) {

    const [error, setError] = useState(null);
    const [categoryIdState, setCategoryIdState] = useState(categoryId);
    const [submitting, setSubmitting] = useState(false);


    const handleCategoryChange = async (e) => {
        setError(null);
        setSubmitting(true);
        const newCategoryId = Number(e.target.value)
        if (newCategoryId === categoryIdState ) return; // No change
        try {
            await patchTransaction(transactionId, { categoryId: newCategoryId });
            setCategoryIdState(newCategoryId);
        } catch (err) {
            setError(err.message);
        } finally {
            setSubmitting(false);
        }
    };

    return ( 
        <div>
            {submitting && <CircularProgress aria-label="Loading…" size={20} />}
            {!submitting &&
                <select value={categoryIdState} onChange={handleCategoryChange}>
                    {[...categoryMap.entries()].map(([id, name]) => (
                        <option key={id} value={id}>
                            {name}
                        </option>
                    ))}
                </select>
            }
            {error && <div className="error">{error}</div>}
        </div>
    );
}