import React, { useState, useEffect, useRef } from 'react';
import Pagination from '../components/Pagination';
import { productImageApi, productApi, uploadApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const ProductImages = () => {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);

    const [tenSanPham, setTenSanPham] = useState('');
    const [laChinhFilter, setLaChinhFilter] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [formData, setFormData] = useState({
        id_san_pham: '',
        la_chinh: false,
        thu_tu: 0,
    });

    const [productSearchText, setProductSearchText] = useState('');
    const [productSuggestions, setProductSuggestions] = useState([]);
    const [showSuggestions, setShowSuggestions] = useState(false);
    const suggestionsRef = useRef(null);

    const [selectedFile, setSelectedFile] = useState(null);
    const [previewUrl, setPreviewUrl] = useState('');
    const [uploading, setUploading] = useState(false);

    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadItems();
    }, [page, tenSanPham, laChinhFilter, sortOrder]);

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (suggestionsRef.current && !suggestionsRef.current.contains(e.target)) {
                setShowSuggestions(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    useEffect(() => {
        return () => {
            if (previewUrl && previewUrl.startsWith('blob:')) {
                URL.revokeObjectURL(previewUrl);
            }
        };
    }, [previewUrl]);

    const loadItems = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (tenSanPham) params.tenSanPham = tenSanPham;
            if (laChinhFilter !== '') params.laChinhFilter = laChinhFilter;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await productImageApi.getAll(params);
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

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (!file) return;
        if (previewUrl && previewUrl.startsWith('blob:')) {
            URL.revokeObjectURL(previewUrl);
        }
        setSelectedFile(file);
        setPreviewUrl(URL.createObjectURL(file));
    };

    const openModal = (item = null) => {
        if (item) {
            setEditingItem(item);
            setFormData({
                id_san_pham: item.id_san_pham || '',
                la_chinh: item.la_chinh || false,
                thu_tu: item.thu_tu ?? 0,
            });
            setProductSearchText(item.sanPham?.ten_sp || `SP #${item.id_san_pham}`);
            setPreviewUrl(item.duong_dan || '');
        } else {
            setEditingItem(null);
            setFormData({ id_san_pham: '', la_chinh: false, thu_tu: 0 });
            setProductSearchText('');
            setPreviewUrl('');
        }
        setSelectedFile(null);
        setProductSuggestions([]);
        setShowSuggestions(false);
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        if (previewUrl && previewUrl.startsWith('blob:')) {
            URL.revokeObjectURL(previewUrl);
        }
        setPreviewUrl('');
        setSelectedFile(null);
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

        if (!editingItem && !selectedFile) {
            setError('Vui lòng chọn file ảnh');
            return;
        }

        try {
            setUploading(true);
            let duongDan = editingItem?.duong_dan || '';

            if (selectedFile) {
                const uploadRes = await uploadApi.uploadImage(selectedFile);
                duongDan = uploadRes.data.url;
            }

            const data = {
                id_san_pham: parseInt(formData.id_san_pham),
                duong_dan: duongDan,
                la_chinh: formData.la_chinh,
                thu_tu: parseInt(formData.thu_tu) || 0,
            };

            if (editingItem) {
                await productImageApi.update(editingItem.id_hinh_anh, data);
            } else {
                await productImageApi.create(data);
            }

            closeModal();
            loadItems();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        } finally {
            setUploading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa hình ảnh này?')) return;
        try {
            await productImageApi.delete(id);
            loadItems();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa hình ảnh');
        }
    };

    const handleSetMain = async (id) => {
        try {
            await productImageApi.setMain(id);
            loadItems();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể đặt ảnh chính');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý hình ảnh sản phẩm</h1>
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
                                            style={{ width: '200px' }}
                                            value={tenSanPham}
                                            onChange={(e) => setTenSanPham(e.target.value)}
                                        />
                                        <select
                                            className="form-control mr-2 mb-2"
                                            value={laChinhFilter}
                                            onChange={(e) => setLaChinhFilter(e.target.value)}
                                        >
                                            <option value="">Tất cả loại</option>
                                            <option value="true">Ảnh chính</option>
                                            <option value="false">Ảnh phụ</option>
                                        </select>
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
                                            <i className="fas fa-plus"></i> Thêm hình ảnh
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
                                                <th style={{ width: '70px' }}>Ảnh</th>
                                                <th>Sản phẩm</th>
                                                <th style={{ width: '100px' }}>Loại</th>
                                                <th style={{ width: '70px' }}>Thứ tự</th>
                                                {isAdmin() && <th style={{ width: '140px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {items.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 6 : 5} className="text-center">
                                                        Không có hình ảnh nào
                                                    </td>
                                                </tr>
                                            ) : (
                                                items.map((item, index) => (
                                                    <tr key={item.id_hinh_anh}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td>
                                                            <img
                                                                src={item.duong_dan}
                                                                alt=""
                                                                style={{ width: '50px', height: '50px', objectFit: 'cover', borderRadius: '4px' }}
                                                                onError={(e) => { e.target.style.display = 'none'; }}
                                                            />
                                                        </td>
                                                        <td>
                                                            <strong>{item.sanPham?.ten_sp || `SP #${item.id_san_pham}`}</strong>
                                                        </td>
                                                        <td>
                                                            {item.la_chinh
                                                                ? <span className="badge badge-warning">Ảnh chính</span>
                                                                : <span className="badge badge-secondary">Ảnh phụ</span>
                                                            }
                                                        </td>
                                                        <td className="text-center">{item.thu_tu}</td>
                                                        {isAdmin() && (
                                                            <td>
                                                                {!item.la_chinh && (
                                                                    <button
                                                                        className="btn btn-sm btn-warning mr-1"
                                                                        title="Đặt làm ảnh chính"
                                                                        onClick={() => handleSetMain(item.id_hinh_anh)}
                                                                    >
                                                                        <i className="fas fa-star"></i>
                                                                    </button>
                                                                )}
                                                                <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(item)}>
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id_hinh_anh)}>
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
                                        <span>Tổng: <strong>{totalCount}</strong> hình ảnh</span>
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
                                        {editingItem ? 'Sửa hình ảnh' : 'Thêm hình ảnh mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form id="imgForm" onSubmit={handleSubmit} style={{ display: 'contents' }}>
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
                                        <div className="form-group">
                                            <label>
                                                {editingItem ? 'Thay ảnh mới (để trống nếu không đổi)' : 'File ảnh'}{' '}
                                                {!editingItem && <span className="text-danger">*</span>}
                                            </label>
                                            <input
                                                type="file"
                                                className="form-control-file"
                                                accept="image/*"
                                                onChange={handleFileChange}
                                            />
                                        </div>
                                        {previewUrl && (
                                            <div className="form-group text-center">
                                                <img
                                                    src={previewUrl}
                                                    alt="Preview"
                                                    style={{ maxWidth: '140px', maxHeight: '120px', objectFit: 'contain', border: '1px solid #ddd', borderRadius: '4px' }}
                                                    onError={(e) => { e.target.style.display = 'none'; }}
                                                />
                                            </div>
                                        )}
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Thứ tự hiển thị</label>
                                                    <input type="number" className="form-control" value={formData.thu_tu}
                                                        onChange={(e) => setFormData({ ...formData, thu_tu: e.target.value })}
                                                        min="0" />
                                                </div>
                                            </div>
                                            <div className="col-6 d-flex align-items-center pt-2">
                                                <div className="custom-control custom-checkbox mt-2">
                                                    <input type="checkbox" className="custom-control-input" id="laChinhCheck"
                                                        checked={formData.la_chinh}
                                                        onChange={(e) => setFormData({ ...formData, la_chinh: e.target.checked })} />
                                                    <label className="custom-control-label" htmlFor="laChinhCheck">Đặt làm ảnh chính</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </form>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-secondary" onClick={closeModal}>Hủy</button>
                                    <button type="submit" form="imgForm" className="btn btn-primary" disabled={uploading}>
                                        {uploading ? (
                                            <><span className="spinner-border spinner-border-sm mr-1"></span>Đang tải lên...</>
                                        ) : (
                                            editingItem ? 'Cập nhật' : 'Thêm mới'
                                        )}
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="modal-backdrop fade show"></div>
                </>
            )}
        </div>
    );
};

export default ProductImages;
