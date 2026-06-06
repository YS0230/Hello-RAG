<script setup lang="ts">
import { onMounted, computed, ref } from 'vue'
import { useUserActionLogStore } from '@/stores/userActionLog'

const store = useUserActionLogStore()
const expandedIds = ref(new Set<number>())

function toggleExpand(id: number) {
  if (expandedIds.value.has(id)) expandedIds.value.delete(id)
  else expandedIds.value.add(id)
}

const resources = ['SystemRule', 'JiebaDictEntry', 'Todo']
const actions = ['Read', 'Create', 'Update', 'Delete']

const totalPages = computed(() => Math.ceil(store.total / store.pageSize))

onMounted(() => store.fetch({ page: 1 }))

function applyFilter(key: string, value: string) {
  store.setFilter(key as any, value)
  store.fetch({ page: 1 })
}

function goPage(p: number) {
  store.fetch({ page: p })
}

function statusClass(code: number) {
  if (code >= 200 && code < 300) return 'text-green-600 dark:text-green-400'
  if (code >= 400 && code < 500) return 'text-amber-600 dark:text-amber-400'
  return 'text-red-600 dark:text-red-400'
}

function actionBadge(action: string) {
  const map: Record<string, string> = {
    Read: 'bg-gray-100 text-gray-600 dark:bg-gray-800/60 dark:text-gray-400',
    Create: 'bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300',
    Update: 'bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-300',
    Delete: 'bg-red-100 text-red-700 dark:bg-red-900/40 dark:text-red-300',
  }
  return map[action] ?? 'bg-muted text-muted-foreground'
}

function formatTime(ts: string) {
  return new Date(ts).toLocaleString('zh-TW', { hour12: false })
}
</script>

<template>
  <div class="space-y-5">
    <!-- 篩選列 -->
    <div class="flex flex-wrap gap-3">
      <select
        class="h-9 rounded-lg border border-border bg-background px-3 text-sm text-text focus:outline-none focus:ring-2 focus:ring-accent/40"
        :value="store.filters.resource ?? ''"
        @change="applyFilter('resource', ($event.target as HTMLSelectElement).value)"
      >
        <option value="">全部資源</option>
        <option v-for="r in resources" :key="r" :value="r">{{ r }}</option>
      </select>

      <select
        class="h-9 rounded-lg border border-border bg-background px-3 text-sm text-text focus:outline-none focus:ring-2 focus:ring-accent/40"
        :value="store.filters.action ?? ''"
        @change="applyFilter('action', ($event.target as HTMLSelectElement).value)"
      >
        <option value="">全部操作</option>
        <option v-for="a in actions" :key="a" :value="a">{{ a }}</option>
      </select>

      <input
        type="date"
        class="h-9 rounded-lg border border-border bg-background px-3 text-sm text-text focus:outline-none focus:ring-2 focus:ring-accent/40"
        :value="store.filters.from ?? ''"
        @change="applyFilter('from', ($event.target as HTMLInputElement).value)"
        placeholder="起始日期"
      />
      <input
        type="date"
        class="h-9 rounded-lg border border-border bg-background px-3 text-sm text-text focus:outline-none focus:ring-2 focus:ring-accent/40"
        :value="store.filters.to ?? ''"
        @change="applyFilter('to', ($event.target as HTMLInputElement).value)"
        placeholder="結束日期"
      />

      <input
        type="text"
        class="h-9 rounded-lg border border-border bg-background px-3 text-sm text-text focus:outline-none focus:ring-2 focus:ring-accent/40"
        :value="store.filters.ip ?? ''"
        @change="applyFilter('ip', ($event.target as HTMLInputElement).value)"
        placeholder="IP 篩選"
      />

      <button
        class="h-9 px-4 rounded-lg border border-border text-sm text-text-muted hover:bg-background-soft transition-colors"
        @click="() => { store.reset(); store.fetch({ page: 1 }) }"
      >
        重置
      </button>

      <button
        class="h-9 px-4 rounded-lg bg-accent text-[0.85rem] font-medium hover:opacity-90 transition-opacity"
        style="color: #F9F8F4;"
        @click="store.fetch({ page: store.page })"
      >
        重新整理
      </button>
    </div>

    <!-- 載入中 / 錯誤 -->
    <div v-if="store.loading" class="py-12 text-center text-text-muted text-sm">載入中…</div>
    <div v-else-if="store.error" class="py-6 text-center text-red-500 text-sm">{{ store.error }}</div>

    <!-- 表格 -->
    <div v-else class="rounded-xl border border-border overflow-hidden">
      <table class="w-full text-sm">
        <thead class="bg-background-soft text-text-muted">
          <tr>
            <th class="px-4 py-3 text-left font-medium">時間</th>
            <th class="px-4 py-3 text-left font-medium">IP</th>
            <th class="px-4 py-3 text-left font-medium">傳入資料</th>
            <th class="px-4 py-3 text-left font-medium">路徑</th>
            <th class="px-4 py-3 text-left font-medium">狀態</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="store.logs.length === 0">
            <td colspan="5" class="px-4 py-10 text-center text-text-muted">尚無紀錄</td>
          </tr>
          <tr
            v-for="log in store.logs"
            :key="log.id"
            class="border-t border-border hover:bg-background-soft/50 transition-colors"
          >
            <td class="px-4 py-3 text-text-muted whitespace-nowrap">{{ formatTime(log.timestamp) }}</td>
            <td class="px-4 py-3 text-text-muted font-mono text-xs">{{ log.ipAddress ?? '—' }}</td>
            <td class="px-4 py-3 text-text-muted font-mono text-xs max-w-[260px]">
              <template v-if="!log.resource">—</template>
              <template v-else-if="log.resource.length <= 60">{{ log.resource }}</template>
              <template v-else>
                <div v-if="!expandedIds.has(log.id)" class="truncate">{{ log.resource }}</div>
                <div v-else class="whitespace-pre-wrap break-all">{{ log.resource }}</div>
                <button
                  class="mt-1 text-accent hover:underline text-[0.75rem]"
                  @click="toggleExpand(log.id)"
                >
                  {{ expandedIds.has(log.id) ? '收合' : '展開' }}
                </button>
              </template>
            </td>
            <td class="px-4 py-3 text-text-muted font-mono text-xs">{{ log.requestPath }}</td>
            <td :class="['px-4 py-3 font-medium', statusClass(log.statusCode)]">{{ log.statusCode }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 分頁 -->
    <div v-if="!store.loading && totalPages > 1" class="flex items-center justify-between text-sm text-text-muted">
      <span>共 {{ store.total }} 筆，第 {{ store.page }} / {{ totalPages }} 頁</span>
      <div class="flex gap-2">
        <button
          class="h-8 px-3 rounded-lg border border-border hover:bg-background-soft transition-colors disabled:opacity-40"
          :disabled="store.page <= 1"
          @click="goPage(store.page - 1)"
        >
          上一頁
        </button>
        <button
          class="h-8 px-3 rounded-lg border border-border hover:bg-background-soft transition-colors disabled:opacity-40"
          :disabled="store.page >= totalPages"
          @click="goPage(store.page + 1)"
        >
          下一頁
        </button>
      </div>
    </div>
  </div>
</template>
