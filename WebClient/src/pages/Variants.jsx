import React, { useState, useEffect, useRef } from 'react';
import Pagination from '../components/Pagination';
import { variantApi, productApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Variants = () => {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);

    const [tenSanPham, setTenSanPham] = useState('');
    const [kichCo, setKichCo] = useState('');
    const [mauSac, setMauSac] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [formData, setFormData] = useState({
        id_san_pham: '',
        kich_co: '',
        mau_sac: '',
        so_luong_ton: 0,
        gia_them: 0,
    });

    const [productSearchText, setProductSearchText] = useState('');
    const [productSuggestions, setProductSuggestions] = useState([]);
    const [showSuggestions, setShowSuggestions] = useState(false);
    const suggestionsRef = useRef(null);

    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadItems();
    }, [page, tenSanPham, kichCo, mauSac, sortOrder]);

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (suggestionsRef.current && !suggestionsRef.current.contains(e.target)) {
                setShowSuggestions(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const loadItems = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (tenSanPham) params.tenSanPham = tenSanPham;
            if (kichCo) params.kichCo = kichCo;
            if (mauSac) params.mauSac = mauSac;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await variantApi.getAll(params);
            setItems(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
        } catch (error) {
            console.error('Failed to load:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadItems();
    };

    const searchProducts = async (keyword) => {
        if (!keyword.trim()) {
            setProductSuggestions([]);
            setShowSuggestions(false);
            return;
        }
        try {
            const res = await productApi.getAll({ keyword, pageSize: 10 });
            setProductSuggestions(res.data.items || []);
            setShowSuggestions(true);
        } catch {
            setProductSuggestions([]);
        }
    };

    const handleProductSearchChange = (e) => {
        const val = e.target.value;
        setProductSearchText(val);
        setFormData(prev => ({ ...prev, id_san_pham: '' }));
        searchProducts(val);
    };

    const selectProduct = (product) => {
        setFormData(prev => ({ ...prev, id_san_pham: product.id_san_pham }));
        setProductSearchText(product.ten_sp);
        setProductSuggestions([]);
        setShowSuggestions(false);
    };

    const openModal = (item = null) => {
        if (item) {
            setEditingItem(item);
            setFormData({
                id_san_pham: item.id_san_pham || '',
                kich_co: item.kich_co || '',
                mau_sac: item.mau_sac || '',
                so_luong_ton: item.so_luong_ton ?? 0,
                gia_them: item.gia_them ?? 0,
            });
            setProductSearchText(item.sanPham?.ten_sp || `SP #${item.id_san_pham}`);
        } else {
            setEditingItem(null);
            setFormData({ id_san_pham: '', kich_co: '', mau_sac: '', so_luong_ton: 0, gia_them: 0 });
            setProductSearchText('');
        }
        setProductSuggestions([]);
        setShowSuggestions(false);
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingItem(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (!formData.id_san_pham) {
            setError('Vui lòng chọn sản phẩm từ danh sách gợi ý');
            return;
        }

        try {
            const data = {
                id_san_pham: parseInt(formData.id_san_pham),
                kich_co: formData.kich_co || null,
                mau_sac: formData.mau_sac || null,
                so_luong_ton: parseInt(formData.so_luong_ton),
                gia_them: parseFloat(formData.gia_them),
            };

            if (editingItem) {
                await variantApi.update(editingItem.id_bien_the, data);
            } else {
                await variantApi.create(data);
            }

            closeModal();
            loadItems();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa biến thể này?')) return;
        try {
            await variantApi.delete(id);
            loadItems();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa biến thể');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý biến thể sản phẩm</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <div className="row">
                                <div className="col-md-9">
                                    <form onSubmit={handleSearch} className="form-inline">
                                        <input
                                            type="text"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Tên sản phẩm..."
                                            style={{ width: '170px' }}
                                            value={tenSanPham}
                                            onChange={(e) => setTenSanPham(e.target.value)}
                                        />
                                        <input
                                            type="text"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Kích cỡ..."
                                            style={{ width: '120px' }}
                                            value={kichCo}
                                            onChange={(e) => setKichCo(e.target.value)}
                                        />
                                        <input
                                            type="text"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Màu sắc..."
                                            style={{ width: '120px' }}
                                            value={mauSac}
                                            onChange={(e) => setMauSac(e.target.value)}
                                        />
                                        <select className="form-control mr-2 mb-2" value={sortOrder} onChange={e => { setSortOrder(e.target.value); setPage(1); }}>
                                            <option value="">Mặc định</option>
                                            <option value="asc">A → Z</option>
                                            <option value="desc">Z → A</option>
                                        </select>
                                        <button type="submit" className="btn btn-primary mb-2">
                                            <i className="fas fa-search"></i> Tìm
                                        </button>
                                    </form>
                                </div>
                                <div className="col-md-3 text-right">
                                    {isAdmin() && (
                                        <button className="btn btn-success" onClick={() => openModal()}>
                                            <i className="fas fa-plus"></i> Thêm biến thể
                                        </button>
                                    )}
                                </div>
                            </div>
                        </div>
                        <div className="card-body">
                            {loading ? (
                                <div className="text-center py-5">
                                    <div className="spinner-border text-primary"></div>
                                </div>
                            ) : (
                                <>
                                    <table className="table table-bordered table-striped">
                                        <thead>
                                            <tr>
                                                <th style={{ width: '60px' }}>STT</th>
                                                <th>Sản phẩm</th>
                                                <th style={{ width: '120px' }}>Kích cỡ</th>
                                                <th style={{ width: '120px' }}>Màu sắc</th>
                                                <th style={{ width: '100px' }}>Tồn kho</th>
                                                <th style={{ width: '130px' }}>Giá thêm</th>
                                                {isAdmin() && <th style={{ width: '110px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {items.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 7 : 6} className="text-center">
                                                        Không có biến thể nào
                                                    </td>
                                                </tr>
                                            ) : (
                                                items.map((item, index) => (
                                                    <tr key={item.id_bien_the}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td>
                                                            <strong>{item.sanPham?.ten_sp || `SP #${item.id_san_pham}`}</strong>
                                                        </td>
                                                        <td>{item.kich_co || '-'}</td>
                                                        <td>{item.mau_sac || '-'}</td>
                                                        <td>
                                                            <span className={`badge ${item.so_luong_ton > 0 ? 'badge-success' : 'badge-danger'}`}>
                                                                {item.so_luong_ton}
                                                            </span>
                                                        </td>
                                                        <td>
                                                            {item.gia_them > 0 ? `+${item.gia_them.toLocaleString('vi-VN')}đ` : '-'}
                                                        </td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(item)}>
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id_bien_the)}>
                                                                    <i className="fas fa-trash"></i>
                                                                </button>
                                                            </td>
                                                        )}
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>

                                    <div className="d-flex justify-content-between align-items-center mt-3">
                                        <span>Tổng: <strong>{totalCount}</strong> biến thể</span>
                                        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {showModal && (
                <>
                    <div className="modal fade show" style={{ display: 'block' }} tabIndex="-1">
                        <div className="modal-dialog modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingItem ? 'Sửa biến thể' : 'Thêm biến thể mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form onSubmit={handleSubmit}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        <div className="form-group" style={{ position: 'relative' }} ref={suggestionsRef}>
                                            <label>Sản phẩm <span className="text-danger">*</span></label>
                                            <input
                                                type="text"
                                                className="form-control"
                                                value={productSearchText}
                                                onChange={handleProductSearchChange}
                                                placeholder="Nhập tên sản phẩm để tìm..."
                                                disabled={!!editingItem}
                                                autoComplete="off"
                                            />
                                            {showSuggestions && productSuggestions.length > 0 && (
                                                <ul className="list-group" style={{ position: 'absolute', zIndex: 1050, width: '100%', maxHeight: '200px', overflowY: 'auto', border: '1px solid #ddd' }}>
                                                    {productSuggestions.map(p => (
                                                        <li
                                                            key={p.id_san_pham}
                                                            className="list-group-item list-group-item-action"
                                                            style={{ cursor: 'pointer' }}
                                                            onMouseDown={() => selectProduct(p)}
                                                        >
                                                            {p.ten_sp}
                                                        </li>
                                                    ))}
                                                </ul>
                                            )}
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Kích cỡ</label>
                                                    <input type="text" className="form-control" value={formData.kich_co}
                                                        onChange={(e) => setFormData({ ...formData, kich_co: e.target.value })}
                                                        placeholder="VD: S, M, L, XL..." />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Màu sắc</label>
                                                    <input type="text" className="form-control" value={formData.mau_sac}
                                                        onChange={(e) => setFormData({ ...formData, mau_sac: e.target.value })}
                                                        placeholder="VD: Đỏ, Xanh..." />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Số lượng tồn <span className="text-danger">*</span></label>
                                                    <input type="number" className="form-control" value={formData.so_luong_ton}
                                                        onChange={(e) => setFormData({ ...formData, so_luong_ton: e.target.value })}
                                                        min="0" required />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Giá thêm (đ)</label>
                                                    <input type="number" className="form-control" value={formData.gia_them}
                                                        onChange={(e) => setFormData({ ...formData, gia_them: e.target.value })}
                                                        min="0" step="1000" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="modal-footer">
                                        <button type="button" className="btn btn-secondary" onClick={closeModal}>Hủy</button>
                                        <button type="submit" className="btn btn-primary">
                                            {editingItem ? 'Cập nhật' : 'Thêm mới'}
                                        </button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div className="modal-backdrop fade show"></div>
                </>
            )}
        </div>
    );
};

export default Variants;
