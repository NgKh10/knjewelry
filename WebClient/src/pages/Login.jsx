import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
    const [ten_dang_nhap, setTenDangNhap] = useState('');  // Sửa tên field
    const [mat_khau, setMatKhau] = useState('');          // Sửa tên field
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        // Sửa: gọi login với đúng tên biến
        const result = await login(ten_dang_nhap, mat_khau);

        if (result.success) {
            navigate('/');
        } else {
            setError(result.message);
        }

        setLoading(false);
    };

    return (
        <div className="login-page" style={{ minHeight: '100vh', background: '#f4f6f9' }}>
            <div className="login-box" style={{ width: '360px', margin: 'auto', paddingTop: '100px' }}>
                <div className="login-logo">
                    <a href="/" style={{ color: '#007bff', fontSize: '28px', fontWeight: 'bold' }}>
                        Trang Sức Bạc
                    </a>
                </div>
                <div className="card">
                    <div className="card-body login-card-body">
                        <p className="login-box-msg">Đăng nhập để bắt đầu</p>

                        {error && (
                            <div className="alert alert-danger alert-dismissible">
                                <button type="button" className="close" onClick={() => setError('')}>
                                    &times;
                                </button>
                                {error}
                            </div>
                        )}

                        <form onSubmit={handleSubmit}>
                            <div className="input-group mb-3">
                                <input
                                    type="text"
                                    className="form-control"
                                    placeholder="Tên đăng nhập"
                                    value={ten_dang_nhap}
                                    onChange={(e) => setTenDangNhap(e.target.value)}
                                    required
                                />
                                <div className="input-group-append">
                                    <div className="input-group-text">
                                        <span className="fas fa-user"></span>
                                    </div>
                                </div>
                            </div>
                            <div className="input-group mb-3">
                                <input
                                    type="password"
                                    className="form-control"
                                    placeholder="Mật khẩu"
                                    value={mat_khau}
                                    onChange={(e) => setMatKhau(e.target.value)}
                                    required
                                />
                                <div className="input-group-append">
                                    <div className="input-group-text">
                                        <span className="fas fa-lock"></span>
                                    </div>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <button
                                        type="submit"
                                        className="btn btn-primary btn-block"
                                        disabled={loading}
                                    >
                                        {loading ? (
                                            <span className="spinner-border spinner-border-sm"></span>
                                        ) : 'Đăng nhập'}
                                    </button>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login;