export const formatDate = (value?: string) => value ? new Intl.DateTimeFormat('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' }).format(new Date(`${value.slice(0,10)}T12:00:00`)) : '—'
export const statusLabel = { Todo: 'Yapılacak', InProgress: 'Devam ediyor', Completed: 'Tamamlandı' } as const
export const priorityLabel = { Low: 'Düşük', Medium: 'Orta', High: 'Yüksek' } as const
export const typeLabel = { Course: 'Kurs', Video: 'Video', Reading: 'Okuma', Exercise: 'Egzersiz', Project: 'Proje', Review: 'Tekrar' } as const
