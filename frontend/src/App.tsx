import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { useAuth } from './context/auth'
import { Layout } from './components/Layout'
import { LoginPage, RegisterPage } from './pages/AuthPages'
import { DashboardPage } from './pages/DashboardPage'
import { TasksPage } from './pages/TasksPage'
import { RoadmapsPage } from './pages/RoadmapsPage'
import { RoadmapDetailPage } from './pages/RoadmapDetailPage'
import { AiPage } from './pages/AiPage'
import { NotificationsPage } from './pages/NotificationsPage'
function Protected() { const { authenticated } = useAuth(); return authenticated ? <Layout/> : <Navigate to="/login" replace/> }
export default function App() { return <BrowserRouter><Routes><Route path="/login" element={<LoginPage/>}/><Route path="/register" element={<RegisterPage/>}/><Route element={<Protected/>}><Route index element={<DashboardPage/>}/><Route path="today" element={<TasksPage mode="today"/>}/><Route path="week" element={<TasksPage mode="week"/>}/><Route path="roadmaps" element={<RoadmapsPage/>}/><Route path="roadmaps/:id" element={<RoadmapDetailPage/>}/><Route path="ai" element={<AiPage/>}/><Route path="notifications" element={<NotificationsPage/>}/></Route><Route path="*" element={<Navigate to="/" replace/>}/></Routes></BrowserRouter> }
