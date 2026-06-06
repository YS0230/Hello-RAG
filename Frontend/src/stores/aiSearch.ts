import { defineStore } from 'pinia'
import { ref } from 'vue'
import { aiSearchApi, type AiSearchResponse } from '@/api/aiSearch'

export const useAiSearchStore = defineStore('aiSearch', () => {
  const answer = ref('')
  const sources = ref<AiSearchResponse['sources']>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const lastQuery = ref('')

  async function search(query: string, topN = 3) {
    if (!query.trim()) return
    loading.value = true
    error.value = null
    lastQuery.value = query
    answer.value = ''
    sources.value = []
    try {
      const res = await aiSearchApi.search({ query, topN })
      answer.value = res.answer
      sources.value = res.sources
    } catch (e) {
      error.value = String(e)
    } finally {
      loading.value = false
    }
  }

  function clear() {
    answer.value = ''
    sources.value = []
    lastQuery.value = ''
    error.value = null
  }

  return { answer, sources, loading, error, lastQuery, search, clear }
})
