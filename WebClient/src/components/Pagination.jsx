import React from 'react';

const Pagination = ({ page, totalPages, onPageChange }) => {
    if (totalPages <= 1) return null;

    const getPageNumbers = () => {
        let start = Math.max(1, page - 2);
        let end = Math.min(totalPages, page + 2);

        if (end - start < 4) {
            if (start === 1) end = Math.min(totalPages, 5);
            else start = Math.max(1, end - 4);
        }

        const pages = [];
        for (let i = start; i <= end; i++) pages.push(i);
        return pages;
    };

    const pages = getPageNumbers();

    return (
        <nav>
            <ul className="pagination mb-0">
                <li className={`page-item ${page === 1 ? 'disabled' : ''}`}>
                    <button className="page-link" onClick={() => onPageChange(page - 1)}>
                        <i className="fas fa-chevron-left"></i>
                    </button>
                </li>
                {pages.map(p => (
                    <li key={p} className={`page-item ${page === p ? 'active' : ''}`}>
                        <button className="page-link" onClick={() => onPageChange(p)}>{p}</button>
                    </li>
                ))}
                <li className={`page-item ${page === totalPages ? 'disabled' : ''}`}>
                    <button className="page-link" onClick={() => onPageChange(page + 1)}>
                        <i className="fas fa-chevron-right"></i>
                    </button>
                </li>
            </ul>
        </nav>
    );
};

export default Pagination;
