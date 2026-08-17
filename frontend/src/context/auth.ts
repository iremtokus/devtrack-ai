import { createContext, useContext } from 'react'
interface UserInfo { displayName: string; email: string }
export interface AuthValue { user: UserInfo | null; authenticated: boolean; login(email: string, password: string): Promise<void>; register(email: string, password: string, displayName: string): Promise<void>; logout(): void }
export const AuthContext = createContext<AuthValue | null>(null)
export const useAuth = () => { const value = useContext(AuthContext); if (!value) throw new Error('AuthProvider eksik'); return value }
