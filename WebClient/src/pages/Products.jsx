import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { productApi, categoryApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    
    // Tìm kiếm
    const [keyword, setKeyword] = useState('');
    const [categoryId, setCategoryId] = useState('');
    const [giaTu, setGiaTu] = useState('');
    const [giaDen, setGiaDen] = useState('');
    const [sortOrder, setSortOrder] = useState('');
    
    // Phân trang
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);
    
    // Modal
    const [showModal, setShowModal] = useState(false);
    const [editingProduct, setEditingProduct] = useState(null);
    const [formData, setFormData] = useState({
        ten_sp: '',
        gia: 0,
        gia_khuyen_mai: '',
        trong_luong: '',
        mo_ta: '',
        id_loai_sp: '',
        id_chat_lieu: 1,
    });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadCategories();
    }, []);

    useEffect(() => {
        loadProducts();
    }, [page, keyword, categoryId, giaTu, giaDen, sortOrder]);

    const loadCategories = async () => {
        try {
            const response = await categoryApi.getAll({ page: 1, pageSize: 100 });
            setCategories(response.data.items || response.data || []);
        } catch (error) {
            console.error('Failed to load categories:', error);
        }
    };

    const loadProducts = async () => {
        setLoading(true);
        try {
            const params = {
                page,
                pageSize,
            };
            if (keyword) params.keyword = keyword;
            if (categoryId) params.loaiId = categoryId;
            if (giaTu) params.giaTu = Number(giaTu);
            if (giaDen) params.giaDen = Number(giaDen);
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await productApi.getAll(params);
            setProducts(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
        } catch (error) {
            console.error('Failed to load products:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadProducts();
    };

    const openModal = (product = null) => {
        if (product) {
            setEditingProduct(product);
            setFormData({
                ten_sp: product.ten_sp,
                gia: product.gia,
                gia_khuyen_mai: product.gia_khuyen_mai || '',
                trong_luong: product.trong_luong || '',
                mo_ta: product.mo_ta || '',
                id_loai_sp: product.id_loai_sp,
                id_chat_lieu: product.id_chat_lieu || 1,
            });
        } else {
            setEditingProduct(null);
            setFormData({
                ten_sp: '',
                gia: 0,
                gia_khuyen_mai: '',
                trong_luong: '',
                mo_ta: '',
                id_loai_sp: categories[0]?.id_loai_sp || '',
                id_chat_lieu: 1,
            });
        }
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingProduct(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const data = {
                ten_sp: formData.ten_sp,
                gia: parseFloat(formData.gia) || 0,
                gia_khuyen_mai: formData.gia_khuyen_mai ? parseFloat(formData.gia_khuyen_mai) : null,
                trong_luong: formData.trong_luong ? parseFloat(formData.trong_luong) : null,
                mo_ta: formData.mo_ta,
                id_loai_sp: parseInt(formData.id_loai_sp),
                id_chat_lieu: parseInt(formData.id_chat_lieu) || 1,
            };

            if (editingProduct) {
                await productApi.update(editingProduct.id_san_pham, data);
            } else {
                await productApi.create(data);
            }

            closeModal();
            loadProducts();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa sản phẩm này?')) return;

        try {
            await productApi.delete(id);
            loadProducts();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa sản phẩm');
        }
    };

    // Format tiền VND
    const formatPrice = (price) => {
        if (price == null) return '-';
        return Number(price).toLocaleString('vi-VN') + ' đ';
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý sản phẩm</h1>
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
                                            placeholder="Tìm tên sản phẩm..."
                                            style={{ width: '200px' }}
                                            value={keyword}
                                            onChange={(e) => setKeyword(e.target.value)}
                                        />
                                        <select
                                            className="form-control mr-2 mb-2"
                                            style={{ width: '180px' }}
                                            value={categoryId}
                                            onChange={(e) => setCategoryId(e.target.value)}
                                        >
                                            <option value="">Tất cả danh mục</option>
                                            {categories.map(cat => (
                                                <option key={cat.id_loai_sp} value={cat.id_loai_sp}>
                                                    {cat.ten_loai}
                                                </option>
                                            ))}
                                        </select>
                                        <input
                                            type="number"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Giá từ"
                                            style={{ width: '120px' }}
                                            value={giaTu}
                                            onChange={(e) => setGiaTu(e.target.value)}
                                        />
                                        <input
                                            type="number"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Giá đến"
                                            style={{ width: '120px' }}
                                            value={giaDen}
                                            onChange={(e) => setGiaDen(e.target.value)}
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
                                            <i className="fas fa-plus"></i> Thêm sản phẩm
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
                                                <th>Tên sản phẩm</th>
                                                <th>Danh mục</th>
                                                <th style={{ width: '120px' }}>Giá</th>
                                                <th style={{ width: '120px' }}>Khuyến mãi</th>
                                                <th style={{ width: '100px' }}>Trạng thái</th>
                                                {isAdmin() && <th style={{ width: '120px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {products.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 7 : 6} className="text-center">
                                                        Không có sản phẩm nào
                                                    </td>
                                                </tr>
                                            ) : (
                                                products.map((product, index) => (
                                                    <tr key={product.id_san_pham}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td>{product.ten_sp}</td>
                                                        <td>
                                                            {product.loaiSanPham?.ten_loai || product.ten_loai || '-'}
                                                        </td>
                                                        <td>{formatPrice(product.gia)}</td>
                                                        <td>
                                                            {product.gia_khuyen_mai ? (
                                                                <span className="text-danger">{formatPrice(product.gia_khuyen_mai)}</span>
                                                            ) : '-'}
                                                        </td>
                                                        <td>
                                                            <span className={`badge ${product.trang_thai === 1 ? 'badge-success' : 'badge-secondary'}`}>
                                                                {product.trang_thai === 1 ? 'Đang bán' : 'Đã ẩn'}
                                                            </span>
                                                        </td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button
                                                                    className="btn btn-sm btn-info mr-1"
                                                                    onClick={() => openModal(product)}
                                                                >
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button
                                                                    className="btn btn-sm btn-danger"
                                                                    onClick={() => handleDelete(product.id_san_pham)}
                                                                >
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
                                        <span>Tổng: <strong>{totalCount}</strong> sản phẩm</span>
                                        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {/* Modal */}
            {showModal && (
                <>
                    <div className="modal fade show" style={{ display: 'block' }} tabIndex="-1">
                        <div className="modal-dialog modal-lg modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingProduct ? 'Sửa sản phẩm' : 'Thêm sản phẩm mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>
                                        <span>&times;</span>
                                    </button>
                                </div>
                                <form onSubmit={handleSubmit}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        <div className="row">
                                            <div className="col-md-8">
                                                <div className="form-group">
                                                    <label>Tên sản phẩm <span className="text-danger">*</span></label>
                                                    <input
                                                        type="text"
                                                        className="form-control"
                                                        value={formData.ten_sp}
                                                        onChange={(e) => setFormData({ ...formData, ten_sp: e.target.value })}
                                                        required
                                                    />
                                                </div>
                                            </div>
                                            <div className="col-md-4">
                                                <div className="form-group">
                                                    <label>Danh mục <span className="text-danger">*</span></label>
                                                    <select
                                                        className="form-control"
                                                        value={formData.id_loai_sp}
                                                        onChange={(e) => setFormData({ ...formData, id_loai_sp: e.target.value })}
                                                        required
                                                    >
                                                        <option value="">Chọn danh mục</option>
                                                        {categories.map(cat => (
                                                            <option key={cat.id_loai_sp} value={cat.id_loai_sp}>
                                                                {cat.ten_loai}
                                                            </option>
                                                        ))}
                                                    </select>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-md-4">
                                                <div className="form-group">
                                                    <label>Giá gốc (VNĐ) <span className="text-danger">*</span></label>
                                                    <input
                                                        type="number"
                                                        className="form-control"
                                                        value={formData.gia}
                                                        onChange={(e) => setFormData({ ...formData, gia: e.target.value })}
                                                        required
                                                        min="0"
                                                    />
                                                </div>
                                            </div>
                                            <div className="col-md-4">
                                                <div className="form-group">
                                                    <label>Giá khuyến mãi (VNĐ)</label>
                                                    <input
                                                        type="number"
                                                        className="form-control"
                                                        value={formData.gia_khuyen_mai}
                                                        onChange={(e) => setFormData({ ...formData, gia_khuyen_mai: e.target.value })}
                                                        min="0"
                                                    />
                                                </div>
                                            </div>
                                            <div className="col-md-4">
                                                <div className="form-group">
                                                    <label>Trọng lượng (gram)</label>
                                                    <input
                                                        type="number"
                                                        step="0.01"
                                                        className="form-control"
                                                        value={formData.trong_luong}
                                                        onChange={(e) => setFormData({ ...formData, trong_luong: e.target.value })}
                                                    />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="form-group">
                                            <label>Mô tả</label>
                                            <textarea
                                                className="form-control"
                                                value={formData.mo_ta}
                                                onChange={(e) => setFormData({ ...formData, mo_ta: e.target.value })}
                                                rows="3"
                                            />
                                        </div>
                                    </div>
                                    <div className="modal-footer">
                                        <button type="button" className="btn btn-secondary" onClick={closeModal}>
                                            Hủy
                                        </button>
                                        <button type="submit" className="btn btn-primary">
                                            {editingProduct ? 'Cập nhật' : 'Thêm mới'}
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

export default Products;