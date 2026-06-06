<script setup lang="ts">
import { ref } from 'vue'
import { useRuleSearchStore } from '@/stores/ruleSearch'
import RuleResultCard from '@/components/RuleResultCard.vue'

const store = useRuleSearchStore()
const query = ref('')
const isEmpty = ref(false)

async function handleSearch() {
  if (!query.value.trim()) {
    isEmpty.value = true
    return
  }
  isEmpty.value = false
  await store.search(query.value.trim())
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <h1
        class="text-xl font-semibold text-heading mb-1"
        style="font-family: 'Playfair Display', Georgia, serif;"
      >
        語意搜尋
      </h1>
      <p class="text-sm text-text-muted">語意搜尋系統規則知識庫</p>
    </div>

    <!-- Search bar -->
    <div class="flex gap-3">
      <div class="flex-1 flex flex-col gap-1">
        <div class="relative">
          <svg
            xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
            fill="none" stroke="currentColor" stroke-width="1.5"
            class="absolute left-4 top-1/2 -translate-y-1/2 text-text-muted pointer-events-none"
          >
            <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
          </svg>
          <input
            v-model="query"
            placeholder="請輸入查詢問題，例如：種植契作作物有哪些注意事項？"
            :class="[
              'w-full pl-10 pr-4 py-3 rounded-full border bg-background-soft text-text text-sm',
              'focus:outline-none focus:ring-2 transition-all duration-300',
              isEmpty
                ? 'border-terracotta focus:ring-terracotta/30'
                : 'border-border focus:ring-sage/40 focus:border-sage',
            ]"
            @keyup.enter="handleSearch"
            @input="isEmpty = false"
          />
        </div>
        <p v-if="isEmpty" class="text-xs text-terracotta pl-4">請輸入查詢問題</p>
      </div>
      <button
        :disabled="store.loading"
        class="px-6 py-3 rounded-full bg-accent text-white text-sm font-medium shrink-0
               hover:opacity-85 disabled:opacity-50 transition-all duration-300"
        @click="handleSearch"
      >
        {{ store.loading ? '搜尋中…' : '搜尋' }}
      </button>
    </div>

    <!-- Error -->
    <div
      v-if="store.error"
      class="px-4 py-3 rounded-2xl border text-sm"
      style="background: rgba(194, 123, 102, 0.08); border-color: rgba(194, 123, 102, 0.3); color: #C27B66;"
    >
      {{ store.error }}
    </div>

    <!-- Skeletons -->
    <div v-if="store.loading" class="flex flex-col gap-3">
      <div
        v-for="i in 3"
        :key="i"
        class="h-32 rounded-3xl bg-background-mute animate-pulse"
      />
    </div>

    <!-- Results -->
    <template v-else-if="store.results.length > 0">
      <p class="text-sm text-text-muted">
        「{{ store.lastQuery }}」共找到 <span class="font-medium text-accent">{{ store.results.length }}</span> 筆相關規則
      </p>
      <div class="flex flex-col gap-4">
        <RuleResultCard
          v-for="result in store.results"
          :key="result.id"
          :result="result"
        />
      </div>
    </template>

    <!-- No results -->
    <div
      v-else-if="store.lastQuery && !store.loading"
      class="text-text-muted text-sm px-1"
    >
      找不到與「{{ store.lastQuery }}」相關的規則
    </div>

    <!-- Empty state -->
    <div
      v-else-if="!store.lastQuery"
      class="flex flex-col items-center justify-center py-20 text-text-muted"
    >
      <div
        class="w-16 h-16 rounded-full flex items-center justify-center mb-5"
        style="background: rgba(45, 58, 49, 0.06);"
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24"
             fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"
             class="opacity-60">
          <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z"/>
          <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12"/>
        </svg>
      </div>
      <p
        class="text-[0.95rem] font-medium text-heading mb-1"
        style="font-family: 'Playfair Display', Georgia, serif;"
      >
        開始搜尋
      </p>
      <p class="text-sm text-text-muted">輸入問題，以語意搜尋相關系統規則</p>
    </div>
  </div>
</template>
