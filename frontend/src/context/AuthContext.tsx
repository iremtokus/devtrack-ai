import { useMemo, useState, type ReactNode } from 'react'
import { api, session } from '../lib/api'
import type { AuthResponse } from '../types'
import { AuthContext, type AuthValue } from './auth'
interface UserInfo { displayName: string; email: string }
const readUser = (): UserInfo | null => { try { return JSON.parse(localStorage.getItem('devtrack_user') || 'null') } catch { return null } }
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(readUser)
  const save = (r: AuthResponse) => { session.set(r.token); const next = { displayName: r.displayName, email: r.email }; localStorage.setItem('devtrack_user', JSON.stringify(next)); setUser(next) }
  const value = useMemo<AuthValue>(() => ({ user, authenticated: Boolean(session.token()), login: async (email, password) => save(await api<AuthResponse>('/api/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) })), register: async (email, password, displayName) => save(await api<AuthResponse>('/api/auth/register', { method: 'POST', body: JSON.stringify({ email, password, displayName }) })), logout: () => { session.clear(); setUser(null) } }), [user])
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
