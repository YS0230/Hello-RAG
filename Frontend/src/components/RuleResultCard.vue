<script setup lang="ts">
import { computed, ref } from 'vue'
import type { RuleSearchResult } from '@/api/systemRule'

const props = defineProps<{ result: RuleSearchResult }>()

const RRF_MAX = 1 / 61 + 0.005

const pct = computed(() => Math.min(100, Math.round((props.result.similarity / RRF_MAX) * 100)))

const badgeClass = computed(() => {
  if (pct.value >= 80) return 'bg-accent text-white'
  if (pct.value >= 50) return 'bg-terracotta text-white'
  return 'bg-background-mute text-text-muted'
})

// tsvector 格式：'詞A':1,2 '詞B':3 → ['詞A', '詞B']
const keywords = computed(() => {
  if (!props.result.searchVector) return []
  return [...props.result.searchVector.matchAll(/'([^']+)'(?::\d+(?:,\d+)*)*/g)]
    .map(m => m[1]?.trim() ?? '')
    .filter(kw => kw.length > 0)
})

const KEYWORD_COLLAPSE_THRESHOLD = 8
const keywordsExpanded = ref(false)
const visibleKeywords = computed(() =>
  keywordsExpanded.value ? keywords.value : keywords.value.slice(0, KEYWORD_COLLAPSE_THRESHOLD)
)
</script>

<template>
  <div
    class="rounded-3xl border border-border bg-background-soft p-6 flex flex-col gap-4
           transition-all duration-500 hover:-translate-y-0.5 cursor-default"
    style="box-shadow: 0 4px 6px -1px rgba(45, 58, 49, 0.05);"
    onmouseenter="this.style.boxShadow='0 10px 15px -3px rgba(45, 58, 49, 0.08)'"
    onmouseleave="this.style.boxShadow='0 4px 6px -1px rgba(45, 58, 49, 0.05)'"
  >
    <div class="flex items-start justify-between gap-3">
      <div class="flex gap-2 flex-wrap">
        <span class="text-xs px-2.5 py-1 rounded-full bg-accent/10 text-accent font-medium">
          {{ result.systemName }}
        </span>
        <span class="text-xs px-2.5 py-1 rounded-full bg-background-mute text-text-muted">
          {{ result.serialNumber }}
        </span>
        <span class="text-xs px-2.5 py-1 rounded-full bg-background-mute text-text-muted">
          {{ result.system }}
        </span>
      </div>
      <div class="flex flex-row items-center gap-2 shrink-0 flex-wrap justify-end">
        <span :class="['text-xs px-2.5 py-1 rounded-full font-semibold', badgeClass]">
          {{ pct }}%
        </span>
        <span
          class="text-xs text-text-muted font-mono cursor-help"
          title="RRF 分數（k=60）&#10;雙路：(1/(60+rank))×vec×0.4 + (1/(60+rank))×txt×0.6 + 0.005&#10;單路：(1/(60+rank))×raw&#10;理論最高 ≈ 0.0214；向量距離門檻 < 0.45"
        >
          RRF {{ result.similarity.toFixed(4) }}
        </span>
        <span class="text-xs text-text-muted font-mono" title="向量餘弦相似度（1 − 距離）">
          vec {{ result.vectorSimilarity.toFixed(4) }}
        </span>
        <span class="text-xs text-text-muted font-mono" title="全文 ts_rank 分數">
          txt {{ result.textScore.toFixed(4) }}
        </span>
      </div>
    </div>

    <p class="text-sm text-text leading-relaxed whitespace-pre-wrap">{{ result.ruleDescription }}</p>

    <div v-if="keywords.length" class="flex flex-wrap gap-1.5 items-center">
      <span class="text-xs text-text-muted">分詞：</span>
      <span
        v-for="kw in visibleKeywords"
        :key="kw"
        class="text-xs px-2 py-0.5 rounded-full bg-clay/40 text-accent font-mono"
      >{{ kw }}</span>
      <button
        v-if="keywords.length > KEYWORD_COLLAPSE_THRESHOLD"
        class="text-xs px-2 py-0.5 rounded-full border border-border text-text-muted hover:text-accent hover:border-accent/40 transition-colors duration-200"
        @click="keywordsExpanded = !keywordsExpanded"
      >
        {{ keywordsExpanded ? '收合 ▲' : `+${keywords.length - KEYWORD_COLLAPSE_THRESHOLD} 展開 ▼` }}
      </button>
    </div>

    <p class="text-xs text-text-muted">填寫人：{{ result.recorder }}</p>
  </div>
</template>
