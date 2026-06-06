<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { todoApi, type TodoItem } from '@/api/todo'
import AppPage from '@/components/AppPage.vue'
import AppCard from '@/components/AppCard.vue'

const todos = ref<TodoItem[]>([])
const newTitle = ref('')

onMounted(async () => {
  todos.value = await todoApi.getAll()
})

async function addTodo() {
  if (!newTitle.value.trim()) return
  const item = await todoApi.create(newTitle.value.trim())
  todos.value.unshift(item)
  newTitle.value = ''
}

async function toggleTodo(item: TodoItem) {
  item.isCompleted = !item.isCompleted
  await todoApi.update(item)
}

async function deleteTodo(id: number) {
  await todoApi.delete(id)
  todos.value = todos.value.filter((t) => t.id !== id)
}
</script>

<template>
  <AppPage>
    <template #title>Todo List</template>

    <AppCard>
      <div class="flex gap-2 mb-5">
        <input
          v-model="newTitle"
          placeholder="新增待辦事項…"
          @keyup.enter="addTodo"
          class="flex-1 px-4 py-2.5 text-sm border border-border rounded-full bg-background text-text
                 placeholder:text-text-muted focus:outline-none focus:ring-2 focus:ring-sage/40
                 focus:border-sage transition-all duration-300"
        />
        <button
          @click="addTodo"
          class="px-5 py-2.5 text-sm font-medium bg-accent text-white rounded-full cursor-pointer
                 hover:opacity-85 transition-all duration-300"
        >
          新增
        </button>
      </div>

      <ul class="list-none p-0 flex flex-col">
        <li
          v-for="todo in todos"
          :key="todo.id"
          class="flex items-center gap-3 py-3 border-b border-border last:border-b-0"
        >
          <input
            type="checkbox"
            :checked="todo.isCompleted"
            @change="toggleTodo(todo)"
            class="w-4 h-4 cursor-pointer accent-accent"
          />
          <span
            :class="
              todo.isCompleted
                ? 'line-through opacity-40 text-text-muted'
                : 'text-text'
            "
            class="flex-1 text-sm"
          >
            {{ todo.title }}
          </span>
          <button
            @click="deleteTodo(todo.id)"
            class="bg-transparent border-0 text-text-muted hover:text-terracotta
                   cursor-pointer text-base leading-none transition-colors duration-300"
          >
            ✕
          </button>
        </li>
      </ul>
    </AppCard>
  </AppPage>
</template>
