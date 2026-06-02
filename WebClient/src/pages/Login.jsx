import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
    const [ten_dang_nhap, setTenDangNhap] = useState('');
    const [mat_khau, setMatKhau] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        const result = await login(ten_dang_nhap, mat_khau);

        if (result.success) {
            navigate('/');
        } else {
            setError(result.message);
        }

        setLoading(false);
    };

    return (
        <div className="login-page" style={{
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #483925 0%, #2e2417 100%)'  // Đổi màu nền
        }}>
            <div className="login-box" style={{ width: '360px', margin: 'auto', paddingTop: '100px' }}>
                <div className="login-logo">
                    <a href="/" style={{
                        color: '#483925',  // màu chữ nâu
                        fontSize: '28px',
                        fontWeight: 'bold',
                        textDecoration: 'none'
                    }}>
                        K&N JEWELRY  // Đổi tên
                    </a>
                </div>
                <div className="card" style={{ borderRadius: '12px', boxShadow: '0 10px 30px rgba(0,0,0,0.15)' }}>
                    <div className="card-body login-card-body">
                        <p className="login-box-msg" style={{ color: '#666', fontSize: '14px' }}>
                            Đăng nhập để bắt đầu
                        </p>

                        {error && (
                            <div className="alert alert-danger alert-dismissible" style={{
                                background: '#fee2e2',
                                color: '#dc2626',
                                borderRadius: '8px',
                                border: '1px solid #fecaca'
                            }}>
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
                                    style={{ borderRadius: '8px 0 0 8px' }}
                                />
                                <div className="input-group-append">
                                    <div className="input-group-text" style={{
                                        background: '#483925',
                                        color: 'white',
                                        border: 'none',
                                        borderRadius: '0 8px 8px 0'
                                    }}>
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
                                    style={{ borderRadius: '8px 0 0 8px' }}
                                />
                                <div className="input-group-append">
                                    <div className="input-group-text" style={{
                                        background: '#483925',
                                        color: 'white',
                                        border: 'none',
                                        borderRadius: '0 8px 8px 0'
                                    }}>
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
                                        style={{
                                            background: '#483925',
                                            border: 'none',
                                            borderRadius: '8px',
                                            padding: '10px',
                                            fontSize: '14px',
                                            fontWeight: '600',
                                            transition: 'all 0.3s'
                                        }}
                                        onMouseEnter={(e) => e.target.style.background = '#2e2417'}
                                        onMouseLeave={(e) => e.target.style.background = '#483925'}
                                    >
                                        {loading ? (
                                            <span className="spinner-border spinner-border-sm"></span>
                                        ) : 'ĐĂNG NHẬP'}
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