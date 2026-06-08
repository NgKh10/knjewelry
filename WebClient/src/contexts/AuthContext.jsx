import React, { createContext, useContext, useState, useEffect } from 'react';
import { authApi } from '../services/api';

const AuthContext = createContext(null);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within AuthProvider');
    return context;
};

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const storedUser = localStorage.getItem('user');
        const token = localStorage.getItem('token');
        if (storedUser && token) {
            setUser(JSON.parse(storedUser));
        }
        setLoading(false);
    }, []);

    const login = async (ten_dang_nhap, mat_khau) => {
        try {
            const response = await authApi.login({ ten_dang_nhap, mat_khau });
            const userData = response.data;
            localStorage.setItem('token', userData.token);
            localStorage.setItem('user', JSON.stringify(userData));
            setUser(userData);
            return { success: true, vai_tro: userData.vai_tro };
        } catch (error) {
            return { success: false, message: error.response?.data?.message || 'Đăng nhập thất bại' };
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
    };

    const isAdmin = () => user?.vai_tro === 'quan_tri';
    const isCustomer = () => user?.vai_tro === 'khach_hang';

    return (
        <AuthContext.Provider value={{
            user,
            login,
            logout,
            isAdmin,
            isCustomer,
            isAuthenticated: !!user,
            loading,
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export default AuthContext;
