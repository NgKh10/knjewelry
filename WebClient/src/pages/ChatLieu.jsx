import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { materialApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const ChatLieu = () => {
    const [chatLieus, setChatLieus] = useState([]);
    const [loading, setLoading] = useState(true);

    // Tìm kiếm
    const [keyword, setKeyword] = useState('');
    const [doTinhKhiet, setDoTinhKhiet] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    // Phân trang
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    // Modal
    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [formData, setFormData] = useState({
        ten_chat_lieu: '',
        do_tinh_khiet: '',
        mo_ta: ''
    });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadChatLieus();
    }, [page, keyword, doTinhKhiet, sortOrder]);

    const loadChatLieus = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (keyword) params.keyword = keyword;
            if (doTinhKhiet) params.doTinhKhiet = doTinhKhiet;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await materialApi.getAll(params);
            setChatLieus(response.data.items || []);
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
        loadChatLieus();
    };

    const openModal = (item = null) => {
        if (item) {
            setEditingItem(item);
            setFormData({
                ten_chat_lieu: item.ten_chat_lieu || '',
                do_tinh_khiet: item.do_tinh_khiet || '',
                mo_ta: item.mo_ta || ''
            });
        } else {
            setEditingItem(null);
            setFormData({ ten_chat_lieu: '', do_tinh_khiet: '', mo_ta: '' });
        }
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

        if (!formData.ten_chat_lieu.trim()) {
            setError('Tên chất liệu không được để trống');
            return;
        }

        try {
            const data = {
                ten_chat_lieu: formData.ten_chat_lieu.trim(),
                do_tinh_khiet: formData.do_tinh_khiet || null,
                mo_ta: formData.mo_ta || null
            };

            if (editingItem) {
                await materialApi.update(editingItem.id_chat_lieu, data);
            } else {
                await materialApi.create(data);
            }

            closeModal();
            loadChatLieus();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa chất liệu này?')) return;
        try {
            await materialApi.delete(id);
            loadChatLieus();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa chất liệu');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý chất liệu</h1>
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
                                            placeholder="Tên chất liệu..."
                                            style={{ width: '200px' }}
                                            value={keyword}
                                            onChange={(e) => setKeyword(e.target.value)}
                                        />
                                        <input
                                            type="text"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Độ tinh khiết..."
                                            style={{ width: '150px' }}
                                            value={doTinhKhiet}
                                            onChange={(e) => setDoTinhKhiet(e.target.value)}
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
                                            <i className="fas fa-plus"></i> Thêm chất liệu
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
                                                <th>Tên chất liệu</th>
                                                <th style={{ width: '150px' }}>Độ tinh khiết</th>
                                                <th>Mô tả</th>
                                                {isAdmin() && <th style={{ width: '110px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {chatLieus.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 5 : 4} className="text-center">
                                                        Không có chất liệu nào
                                                    </td>
                                                </tr>
                                            ) : (
                                                chatLieus.map((item, index) => (
                                                    <tr key={item.id_chat_lieu}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td><strong>{item.ten_chat_lieu}</strong></td>
                                                        <td>
                                                            {item.do_tinh_khiet ? (
                                                                <span className="badge badge-info">{item.do_tinh_khiet}</span>
                                                            ) : '-'}
                                                        </td>
                                                        <td>{item.mo_ta || '-'}</td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(item)}>
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id_chat_lieu)}>
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
                                        <span>Tổng: <strong>{totalCount}</strong> chất liệu</span>
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
                        <div className="modal-dialog modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingItem ? 'Sửa chất liệu' : 'Thêm chất liệu mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form onSubmit={handleSubmit}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        <div className="form-group">
                                            <label>Tên chất liệu <span className="text-danger">*</span></label>
                                            <input type="text" className="form-control" value={formData.ten_chat_lieu}
                                                onChange={(e) => setFormData({ ...formData, ten_chat_lieu: e.target.value })}
                                                placeholder="VD: Bạc 925, Bạc 999..." required />
                                        </div>
                                        <div className="form-group">
                                            <label>Độ tinh khiết</label>
                                            <input type="text" className="form-control" value={formData.do_tinh_khiet}
                                                onChange={(e) => setFormData({ ...formData, do_tinh_khiet: e.target.value })}
                                                placeholder="VD: 92.5%, 99.9%" />
                                        </div>
                                        <div className="form-group">
                                            <label>Mô tả</label>
                                            <textarea className="form-control" rows="3" value={formData.mo_ta}
                                                onChange={(e) => setFormData({ ...formData, mo_ta: e.target.value })}
                                                placeholder="Mô tả chi tiết..." />
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

export default ChatLieu;
