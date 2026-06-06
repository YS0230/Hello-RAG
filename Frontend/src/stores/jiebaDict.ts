import { ref } from 'vue'
import { defineStore } from 'pinia'
import { jiebaDictApi, type JiebaDictEntry, type JiebaDictPayload } from '@/api/jiebaDict'

export const useJiebaDictStore = defineStore('jiebaDict', () => {
  const entries = ref<JiebaDictEntry[]>([])
  const loading = ref(false)
  const rebuilding = ref(false)
  const error = ref<string | null>(null)
  const rebuildMessage = ref<string | null>(null)

  async function fetchAll() {
    loading.value = true
    error.value = null
    try {
      entries.value = await jiebaDictApi.getAll()
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      loading.value = false
    }
  }

  async function create(payload: JiebaDictPayload) {
    const entry = await jiebaDictApi.create(payload)
    entries.value.push(entry)
    entries.value.sort((a, b) => a.word.localeCompare(b.word))
  }

  async function update(id: number, payload: JiebaDictPayload) {
    await jiebaDictApi.update(id, payload)
    const idx = entries.value.findIndex(e => e.id === id)
    const existing = idx !== -1 ? entries.value[idx] : undefined
    if (existing) Object.assign(existing, payload)
    entries.value.sort((a, b) => a.word.localeCompare(b.word))
  }

  async function remove(id: number) {
    await jiebaDictApi.delete(id)
    entries.value = entries.value.filter(e => e.id !== id)
  }

  async function rebuild() {
    rebuilding.value = true
    rebuildMessage.value = null
    error.value = null
    try {
      const res = await jiebaDictApi.rebuild()
      rebuildMessage.value = res.message
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      rebuilding.value = false
    }
  }

  return { entries, loading, rebuilding, error, rebuildMessage, fetchAll, create, update, remove, rebuild }
})
