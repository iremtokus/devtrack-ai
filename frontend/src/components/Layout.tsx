import { useEffect, useState } from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { Bell, Bot, CalendarDays, CheckSquare, LayoutDashboard, LogOut, Map, Menu, X } from 'lucide-react'
import { useAuth } from '../context/auth'
import { api } from '../lib/api'
import type { Reminder } from '../types'
const links = [
  { to: '/', label: 'Dashboard', icon: LayoutDashboard }, { to: '/today', label: 'Bugün', icon: CheckSquare },
  { to: '/week', label: 'Bu Hafta', icon: CalendarDays }, { to: '/roadmaps', label: 'Roadmapler', icon: Map },
  { to: '/ai', label: 'AI Asistan', icon: Bot }, { to: '/notifications', label: 'Bildirimler', icon: Bell },
]
export function Layout() {
  const [open, setOpen] = useState(false); const [unread, setUnread] = useState(0); const { user, logout } = useAuth(); const navigate = useNavigate()
  useEffect(() => { api<Reminder[]>('/api/reminders?unreadOnly=true').then(x => setUnread(x.length)).catch(() => {}) }, [])
  const exit = () => { logout(); navigate('/login') }
  return <div className="min-h-screen lg:flex"><header className="sticky top-0 z-40 flex h-16 items-center justify-between border-b bg-white px-4 lg:hidden"><span className="text-lg font-bold text-ink">DevTrack <span className="text-brand-600">AI</span></span><button onClick={() => setOpen(!open)} aria-label="Menü" className="p-2">{open ? <X/> : <Menu/>}</button></header>
    <aside className={`${open ? 'block' : 'hidden'} fixed inset-x-0 top-16 z-30 border-b bg-white p-4 lg:sticky lg:top-0 lg:block lg:h-screen lg:w-64 lg:border-b-0 lg:border-r lg:p-5`}>
      <div className="mb-8 hidden text-xl font-bold text-ink lg:block">DevTrack <span className="text-brand-600">AI</span></div>
      <nav className="grid grid-cols-2 gap-1 lg:block">{links.map(({to,label,icon:Icon}) => <NavLink key={to} to={to} end={to === '/'} onClick={() => setOpen(false)} className={({isActive}) => `mb-1 flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium ${isActive ? 'bg-brand-50 text-brand-700' : 'text-slate-600 hover:bg-slate-50'}`}><Icon size={18}/><span>{label}</span>{label === 'Bildirimler' && unread > 0 && <span className="ml-auto rounded-full bg-red-500 px-2 py-0.5 text-xs text-white">{unread}</span>}</NavLink>)}</nav>
      <div className="mt-5 border-t pt-4 lg:absolute lg:bottom-5 lg:left-5 lg:right-5"><p className="truncate text-sm font-semibold">{user?.displayName}</p><p className="truncate text-xs text-slate-500">{user?.email}</p><button onClick={exit} className="mt-3 flex items-center gap-2 text-sm font-medium text-slate-600 hover:text-red-600"><LogOut size={17}/> Çıkış</button></div>
    </aside><main className="min-w-0 flex-1"><div className="mx-auto max-w-7xl p-4 sm:p-6 lg:p-8"><Outlet/></div></main></div>
}
