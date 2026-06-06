import { defineStore } from 'pinia'
import { ref } from 'vue'
import { ruleSearchApi, type RuleSearchResult } from '@/api/systemRule'

export const useRuleSearchStore = defineStore('ruleSearch', () => {
  const results = ref<RuleSearchResult[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const lastQuery = ref('')

  async function search(query: string, topN = 5) {
    if (!query.trim()) return
    loading.value = true
    error.value = null
    lastQuery.value = query
    try {
      results.value = await ruleSearchApi.search({ query, topN })
    } catch (e) {
      error.value = String(e)
      results.value = []
    } finally {
      loading.value = false
    }
  }

  function clear() {
    results.value = []
    lastQuery.value = ''
    error.value = null
  }

  return { results, loading, error, lastQuery, search, clear }
})
