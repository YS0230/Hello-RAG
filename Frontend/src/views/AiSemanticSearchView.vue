<script setup lang="ts">
import { ref, computed } from 'vue'
import { marked } from 'marked'
import { useAiSearchStore } from '@/stores/aiSearch'
import RuleResultCard from '@/components/RuleResultCard.vue'

const store = useAiSearchStore()
const query = ref('')
const isEmpty = ref(false)

const renderedAnswer = computed(() => marked.parse(store.answer) as string)

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
        AI 語意搜尋
      </h1>
      <p class="text-sm text-text-muted">輸入問題，AI 整合知識庫資料為您生成回答</p>
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
            placeholder="請輸入問題，例如：種植契作作物有哪些注意事項？"
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
        {{ store.loading ? 'AI 分析中…' : '搜尋' }}
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
    <template v-if="store.loading">
      <div class="h-48 rounded-3xl bg-background-mute animate-pulse" />
      <div class="flex flex-col gap-3">
        <div v-for="i in 3" :key="i" class="h-28 rounded-3xl bg-background-mute animate-pulse" />
      </div>
    </template>

    <!-- Results -->
    <template v-else-if="store.answer">
      <!-- AI Answer card -->
      <div
        class="rounded-3xl border border-border bg-background-soft p-6 flex flex-col gap-3"
        style="box-shadow: 0 4px 6px -1px rgba(45, 58, 49, 0.05);"
      >
        <div class="flex items-center gap-2">
          <div class="w-6 h-6 rounded-full bg-accent flex items-center justify-center shrink-0">
            <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24"
                 fill="none" stroke="#F9F8F4" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z"/>
              <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12"/>
            </svg>
          </div>
          <span
            class="text-[0.9rem] font-semibold text-heading"
            style="font-family: 'Playfair Display', Georgia, serif;"
          >
            AI 回應
          </span>
        </div>
        <div class="prose-answer text-sm text-text leading-relaxed" v-html="renderedAnswer" />
      </div>

      <!-- Sources -->
      <div class="flex flex-col gap-3">
        <p class="text-sm text-text-muted">
          來源知識 · 共參考 <span class="font-medium text-accent">{{ store.sources.length }}</span> 筆資料
        </p>
        <div class="flex flex-col gap-4">
          <RuleResultCard
            v-for="source in store.sources"
            :key="source.id"
            :result="source"
          />
        </div>
      </div>
    </template>

    <!-- No results -->
    <div
      v-else-if="store.lastQuery && !store.loading"
      class="text-text-muted text-sm px-1"
    >
      找不到與「{{ store.lastQuery }}」相關的資料
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
          <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
        </svg>
      </div>
      <p
        class="text-[0.95rem] font-medium text-heading mb-1"
        style="font-family: 'Playfair Display', Georgia, serif;"
      >
        開始 AI 搜尋
      </p>
      <p class="text-sm text-text-muted">輸入問題，AI 會整合知識庫資料為您生成回答並標示來源</p>
    </div>
  </div>
</template>

<style scoped>
.prose-answer :deep(h1),
.prose-answer :deep(h2),
.prose-answer :deep(h3) {
  font-family: 'Playfair Display', Georgia, serif;
  font-weight: 600;
  color: var(--color-heading);
  margin: 1em 0 0.4em;
  line-height: 1.4;
}
.prose-answer :deep(h1) { font-size: 1.1rem; }
.prose-answer :deep(h2) { font-size: 1rem; }
.prose-answer :deep(h3) { font-size: 0.95rem; }
.prose-answer :deep(p) { margin: 0.5em 0; }
.prose-answer :deep(ul),
.prose-answer :deep(ol) {
  padding-left: 1.4em;
  margin: 0.4em 0;
}
.prose-answer :deep(li) { margin: 0.25em 0; }
.prose-answer :deep(strong) { font-weight: 600; color: var(--color-heading); }
.prose-answer :deep(code) {
  font-family: ui-monospace, monospace;
  font-size: 0.85em;
  background: rgba(45, 58, 49, 0.08);
  padding: 0.15em 0.35em;
  border-radius: 4px;
}
.prose-answer :deep(pre) {
  background: rgba(45, 58, 49, 0.06);
  padding: 0.75em 1em;
  border-radius: 12px;
  overflow-x: auto;
  margin: 0.5em 0;
}
.prose-answer :deep(pre code) {
  background: none;
  padding: 0;
}
.prose-answer :deep(blockquote) {
  border-left: 3px solid var(--color-sage);
  margin: 0.5em 0;
  padding-left: 1em;
  color: var(--color-text-muted);
}
.prose-answer :deep(*:first-child) { margin-top: 0; }
.prose-answer :deep(*:last-child) { margin-bottom: 0; }
</style>
