import { defineStore } from 'pinia'
import { ref } from 'vue'
import { systemRuleApi, type SystemRule, type SystemRulePayload } from '@/api/systemRule'

export const useSystemRuleStore = defineStore('systemRule', () => {
  const rules = ref<SystemRule[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAll() {
    loading.value = true
    error.value = null
    try {
      rules.value = await systemRuleApi.getAll()
    } catch (e) {
      error.value = String(e)
    } finally {
      loading.value = false
    }
  }

  async function create(payload: SystemRulePayload) {
    const created = await systemRuleApi.create(payload)
    rules.value.unshift(created)
  }

  async function update(id: number, payload: SystemRulePayload) {
    await systemRuleApi.update(id, payload)
    await fetchAll()
  }

  async function remove(id: number) {
    await systemRuleApi.delete(id)
    rules.value = rules.value.filter(r => r.id !== id)
  }

  return { rules, loading, error, fetchAll, create, update, remove }
})
