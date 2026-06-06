<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { rateLimitApi, type IpDailyCount } from '@/api/rateLimit'

const items = ref<IpDailyCount[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const dailyLimit = 100

async function load() {
  loading.value = true
  error.value = null
  try {
    items.value = await rateLimitApi.getDaily()
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

function usageClass(count: number) {
  const ratio = count / dailyLimit
  if (ratio >= 1) return 'text-red-600 dark:text-red-400'
  if (ratio >= 0.8) return 'text-amber-600 dark:text-amber-400'
  return 'text-green-600 dark:text-green-400'
}

function barClass(count: number) {
  const ratio = count / dailyLimit
  if (ratio >= 1) return 'bg-red-500'
  if (ratio >= 0.8) return 'bg-amber-400'
  return 'bg-green-500'
}

function barWidth(count: number) {
  return Math.min((count / dailyLimit) * 100, 100) + '%'
}

onMounted(load)
</script>

<template>
  <div class="space-y-5">
    <div class="flex items-center justify-between">
      <p class="text-sm text-text-muted">每日上限：{{ dailyLimit }} 次</p>
      <button
        class="h-9 px-4 rounded-lg bg-accent text-[0.85rem] font-medium hover:opacity-90 transition-opacity"
        style="color: #F9F8F4;"
        @click="load"
      >
        重新整理
      </button>
    </div>

    <div v-if="loading" class="py-12 text-center text-text-muted text-sm">載入中…</div>
    <div v-else-if="error" class="py-6 text-center text-red-500 text-sm">{{ error }}</div>

    <div v-else class="rounded-xl border border-border overflow-hidden">
      <table class="w-full text-sm">
        <thead class="bg-background-soft text-text-muted">
          <tr>
            <th class="px-4 py-3 text-left font-medium">IP</th>
            <th class="px-4 py-3 text-left font-medium w-48">使用次數</th>
            <th class="px-4 py-3 text-left font-medium">進度</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="items.length === 0">
            <td colspan="3" class="px-4 py-10 text-center text-text-muted">今日尚無紀錄</td>
          </tr>
          <tr
            v-for="item in items"
            :key="item.ip"
            class="border-t border-border hover:bg-background-soft/50 transition-colors"
          >
            <td class="px-4 py-3 font-mono text-xs text-text">{{ item.ip }}</td>
            <td :class="['px-4 py-3 font-medium tabular-nums', usageClass(item.count)]">
              {{ item.count }} / {{ dailyLimit }}
            </td>
            <td class="px-4 py-3">
              <div class="h-2 w-full max-w-[200px] rounded-full bg-border overflow-hidden">
                <div
                  :class="['h-full rounded-full transition-all', barClass(item.count)]"
                  :style="{ width: barWidth(item.count) }"
                />
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
