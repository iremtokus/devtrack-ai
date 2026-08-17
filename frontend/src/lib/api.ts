const API_BASE = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000').replace(/\/$/, '')
const TOKEN_KEY = 'devtrack_token'
export class ApiError extends Error { status: number; constructor(message: string, status: number) { super(message); this.status = status } }
export const session = { token: () => localStorage.getItem(TOKEN_KEY), set: (token: string) => localStorage.setItem(TOKEN_KEY, token), clear: () => { localStorage.removeItem(TOKEN_KEY); localStorage.removeItem('devtrack_user') } }
export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers); if (options.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json')
  const token = session.token(); if (token) headers.set('Authorization', `Bearer ${token}`)
  let response: Response; try { response = await fetch(`${API_BASE}${path}`, { ...options, headers }) } catch { throw new ApiError('Sunucuya ulaşılamıyor. Backend bağlantısını kontrol et.', 0) }
  if (response.status === 401) { session.clear(); if (location.pathname !== '/login') location.assign('/login'); throw new ApiError('Oturumun sona erdi. Lütfen tekrar giriş yap.', 401) }
  if (!response.ok) { let message = 'İşlem sırasında bir hata oluştu.'; try { const data = await response.json(); message = data.title || Object.values(data.errors || {}).flat().join(' ') || message } catch { /* boş yanıt */ } throw new ApiError(message, response.status) }
  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}
