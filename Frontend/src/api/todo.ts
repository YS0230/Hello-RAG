export interface TodoItem {
  id: number
  title: string
  isCompleted: boolean
  createdAt: string
}

const BASE = '/api/todo'

export const todoApi = {
  getAll: (): Promise<TodoItem[]> => fetch(BASE).then((r) => r.json()),

  create: (title: string): Promise<TodoItem> =>
    fetch(BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title, isCompleted: false }),
    }).then((r) => r.json()),

  update: (item: TodoItem): Promise<void> =>
    fetch(`${BASE}/${item.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item),
    }).then(() => undefined),

  delete: (id: number): Promise<void> =>
    fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(() => undefined),
}
