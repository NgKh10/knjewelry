import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { categoryApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Categories = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [parentCategories, setParentCategories] = useState([]);

    // Tìm kiếm
    const [keyword, setKeyword] = useState('');
    const [idLoaiCha, setIdLoaiCha] = useState('');
    const [sortOrder, setSortOrder] = useState('');
    
    // Phân trang
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);
    
    // Modal
    const [showModal, setShowModal] = useState(false);
    const [editingCategory, setEditingCategory] = useState(null);
    const [formData, setFormData] = useState({
        ten_loai: '',
        mo_ta: '',
        id_loai_cha: null,
        thu_tu: 0,
    });
    const [error, setError] = useState('');
    
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadParentCategories();
    }, []);

    useEffect(() => {
        loadCategories();
    }, [page, keyword, idLoaiCha, sortOrder]);

    const loadParentCategories = async () => {
        try {
            const response = await categoryApi.getAll({ idLoaiCha: 0, pageSize: 100 });
            setParentCategories(response.data?.items || []);
        } catch (error) {
            console.error('Failed to load parent categories:', error);
        }
    };

    const loadCategories = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (keyword) params.keyword = keyword;
            if (idLoaiCha !== '') params.idLoaiCha = idLoaiCha;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await categoryApi.getAll(params);
            setCategories(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
        } catch (error) {
            console.error('Failed to load categories:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
    };

    const openModal = (category = null) => {
        if (category) {
            setEditingCategory(category);
            setFormData({
                ten_loai: category.ten_loai,
                mo_ta: category.mo_ta || '',
                id_loai_cha: category.id_loai_cha || null,
                thu_tu: category.thu_tu || 0,
            });
        } else {
            setEditingCategory(null);
            setFormData({ ten_loai: '', mo_ta: '', id_loai_cha: null, thu_tu: 0 });
        }
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingCategory(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const data = {
                ten_loai: formData.ten_loai,
                mo_ta: formData.mo_ta,
                id_loai_cha: formData.id_loai_cha || null,
                thu_tu: formData.thu_tu || 0,
            };

            if (editingCategory) {
                await categoryApi.update(editingCategory.id_loai_sp, data);
            } else {
                await categoryApi.create(data);
            }
            closeModal();
            loadCategories();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Xóa danh mục này?')) return;
        try {
            await categoryApi.delete(id);
            loadCategories();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <h1 className="m-0">Quản lý danh mục</h1>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <form onSubmit={handleSearch} className="form-inline">
                                <input
                                    type="text"
                                    className="form-control mr-2"
                                    placeholder="Tìm kiếm..."
                                    value={keyword}
                                    onChange={(e) => setKeyword(e.target.value)}
                                />
                                <select
                                    className="form-control mr-2"
                                    value={idLoaiCha}
                                    onChange={(e) => setIdLoaiCha(e.target.value)}
                                >
                                    <option value="">Tất cả danh mục</option>
                                    <option value="0">Danh mục cấp 1</option>
                                    {parentCategories.map(cat => (
                                        <option key={cat.id_loai_sp} value={cat.id_loai_sp}>
                                            {cat.ten_loai}
                                        </option>
                                    ))}
                                </select>
                                <select className="form-control mr-2" value={sortOrder} onChange={e => { setSortOrder(e.target.value); setPage(1); }}>
                                    <option value="">Mặc định</option>
                                    <option value="asc">A → Z</option>
                                    <option value="desc">Z → A</option>
                                </select>
                                <button type="submit" className="btn btn-primary mr-2">
                                    <i className="fas fa-search"></i> Tìm
                                </button>
                                {isAdmin() && (
                                    <button type="button" className="btn btn-success" onClick={() => openModal()}>
                                        <i className="fas fa-plus"></i> Thêm
                                    </button>
                                )}
                            </form>
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
                                                <th>Tên danh mục</th>
                                                <th style={{ width: '160px' }}>Danh mục cha</th>
                                                <th>Mô tả</th>
                                                {isAdmin() && <th style={{ width: '100px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {categories.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 5 : 4} className="text-center">
                                                        Không có dữ liệu
                                                    </td>
                                                </tr>
                                            ) : (
                                                categories.map((cat, index) => (
                                                    <tr key={cat.id_loai_sp}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td>{cat.ten_loai}</td>
                                                        <td>
                                                            {cat.id_loai_cha
                                                                ? (parentCategories.find(p => p.id_loai_sp === cat.id_loai_cha)?.ten_loai || `#${cat.id_loai_cha}`)
                                                                : <span className="badge badge-secondary">Gốc</span>
                                                            }
                                                        </td>
                                                        <td>{cat.mo_ta || '-'}</td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(cat)}>
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(cat.id_loai_sp)}>
                                                                    <i className="fas fa-trash"></i>
                                                                </button>
                                                            </td>
                                                        )}
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>

                                    <div className="d-flex justify-content-between">
                                        <span>Tổng: {totalCount}</span>
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
                    <div className="modal fade show" style={{ display: 'block' }}>
                        <div className="modal-dialog modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5>{editingCategory ? 'Sửa' : 'Thêm'} danh mục</h5>
                                    <button className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form onSubmit={handleSubmit}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        <div className="form-group">
                                            <label>Tên danh mục <span className="text-danger">*</span></label>
                                            <input type="text" className="form-control" value={formData.ten_loai}
                                                onChange={(e) => setFormData({...formData, ten_loai: e.target.value})} required />
                                        </div>
                                        <div className="form-group">
                                            <label>Danh mục cha</label>
                                            <select className="form-control"
                                                value={formData.id_loai_cha ?? ''}
                                                onChange={(e) => setFormData({...formData, id_loai_cha: e.target.value ? parseInt(e.target.value) : null})}
                                            >
                                                <option value="">Không có (danh mục gốc)</option>
                                                {parentCategories.map(cat => (
                                                    <option key={cat.id_loai_sp} value={cat.id_loai_sp}>{cat.ten_loai}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <div className="form-group">
                                            <label>Mô tả</label>
                                            <textarea className="form-control" value={formData.mo_ta}
                                                onChange={(e) => setFormData({...formData, mo_ta: e.target.value})} rows="3" />
                                        </div>
                                    </div>
                                    <div className="modal-footer">
                                        <button type="button" className="btn btn-secondary" onClick={closeModal}>Hủy</button>
                                        <button type="submit" className="btn btn-primary">
                                            {editingCategory ? 'Cập nhật' : 'Thêm'}
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

export default Categories;