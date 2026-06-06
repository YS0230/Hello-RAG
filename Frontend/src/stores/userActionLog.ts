import { defineStore } from 'pinia'
import { ref } from 'vue'
import { userActionLogApi, type LogQueryParams, type UserActionLog } from '@/api/userActionLog'

export const useUserActionLogStore = defineStore('userActionLog', () => {
  const logs = ref<UserActionLog[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const page = ref(1)
  const pageSize = ref(20)
  const filters = ref<LogQueryParams>({})

  async function fetch(params: LogQueryParams = {}) {
    loading.value = true
    error.value = null
    try {
      const result = await userActionLogApi.query({ pageSize: pageSize.value, ...filters.value, ...params })
      logs.value = result.items
      total.value = result.total
      page.value = result.page
    } catch (e: any) {
      error.value = e.message
    } finally {
      loading.value = false
    }
  }

  function setFilter(key: keyof LogQueryParams, value: string) {
    filters.value = { ...filters.value, [key]: value || undefined }
  }

  function reset() {
    filters.value = {}
    page.value = 1
  }

  return { logs, total, loading, error, page, pageSize, filters, fetch, setFilter, reset }
})
