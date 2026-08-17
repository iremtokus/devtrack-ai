export type TaskStatus = 'Todo' | 'InProgress' | 'Completed'
export type TaskType = 'Course' | 'Video' | 'Reading' | 'Exercise' | 'Project' | 'Review'
export type Priority = 'Low' | 'Medium' | 'High'
export interface AuthResponse { token: string; displayName: string; email: string }
export interface Task { id: string; title: string; description?: string; type: TaskType; status: TaskStatus; priority: Priority; plannedDate: string; deadline?: string; estimatedMinutes: number; completedAtUtc?: string; resourceUrl?: string; roadmapWeekId: string; isOverdue: boolean }
export interface Week { id: string; weekNumber: number; title: string; progress: number; tasks: Task[] }
export interface Roadmap { id: string; title: string; description?: string; durationWeeks: number; startDate: string; progress: number; weeks: Week[] }
export interface Dashboard { today: Task[]; thisWeek: Task[]; overdue: Task[]; recentlyCompleted: Task[]; upcomingDeadlines: Task[]; completed: number; remaining: number; progress: number; streak: number; reminders: string[] }
export interface Reminder { id: string; message: string; type: 'DueToday' | 'Overdue'; isRead: boolean; createdAtUtc: string; learningTaskId?: string }
export interface RescheduleProposal { taskId: string; taskTitle: string; proposedPlannedDate: string; proposedDeadline?: string; reason: string }
export interface AiSuggestion { recommendationId: string; action: string; title: string; summary: string; steps: string[]; rescheduleProposals: RescheduleProposal[]; providerIdentifier: string; disclaimer: string }
export interface TaskInput { title: string; description: string; type: TaskType; status: TaskStatus; priority: Priority; plannedDate: string; deadline: string | null; estimatedMinutes: number; resourceUrl: string | null }
