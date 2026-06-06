<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSystemRuleStore } from '@/stores/systemRule'
import RuleFormModal from '@/components/RuleFormModal.vue'
import type { SystemRule, SystemRulePayload } from '@/api/systemRule'

const store = useSystemRuleStore()
const expandedIds = ref(new Set<number>())
const keyword = ref('')

const filteredRules = computed(() => {
  const kw = keyword.value.trim().toLowerCase()
  if (!kw) return store.rules
  return store.rules.filter(r =>
    [r.systemName, r.serialNumber, r.system, r.ruleDescription, r.recorder]
      .some(v => v?.toLowerCase().includes(kw))
  )
})

function toggleExpand(id: number) {
  if (expandedIds.value.has(id)) expandedIds.value.delete(id)
  else expandedIds.value.add(id)
  expandedIds.value = new Set(expandedIds.value)
}
const showModal = ref(false)
const editingRule = ref<SystemRule | null>(null)
const saving = ref(false)
const actionError = ref<string | null>(null)
const actionSuccess = ref<string | null>(null)

onMounted(() => store.fetchAll())

function notify(msg: string, isError = false) {
  actionError.value = isError ? msg : null
  actionSuccess.value = isError ? null : msg
  if (!isError) setTimeout(() => { actionSuccess.value = null }, 4000)
}

function openCreate() {
  editingRule.value = null
  showModal.value = true
}

function openEdit(rule: SystemRule) {
  editingRule.value = rule
  showModal.value = true
}

async function handleSaved(payload: SystemRulePayload) {
  saving.value = true
  try {
    if (editingRule.value) {
      await store.update(editingRule.value.id, payload)
      notify('規則已更新')
    } else {
      await store.create(payload)
      notify('規則已新增')
    }
  } catch (e) {
    notify(String(e instanceof Error ? e.message : e), true)
  } finally {
    saving.value = false
  }
}

async function handleDelete(rule: SystemRule) {
  if (!confirm(`確定刪除「${rule.systemName} - ${rule.serialNumber}」？`)) return
  try {
    await store.remove(rule.id)
    notify('規則已刪除')
  } catch (e) {
    notify(String(e instanceof Error ? e.message : e), true)
  }
}

function formatDate(dt: string) {
  return new Date(dt).toLocaleDateString('zh-TW')
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1
          class="text-xl font-semibold text-heading mb-0.5"
          style="font-family: 'Playfair Display', Georgia, serif;"
        >
          知識庫
        </h1>
        <p class="text-sm text-text-muted">管理系統知識規則庫</p>
      </div>
      <div class="flex items-center gap-3">
        <input
          v-model="keyword"
          type="text"
          placeholder="關鍵字篩選…"
          class="px-4 py-2 rounded-full border border-border text-sm bg-background
                 focus:outline-none focus:border-accent/50 transition-colors duration-200"
        />
        <button
          class="px-5 py-2.5 rounded-full bg-accent text-white text-sm font-medium
                 hover:opacity-85 transition-all duration-300"
          @click="openCreate"
        >
          ＋ 新增規則
        </button>
      </div>
    </div>

    <!-- Alerts -->
    <div
      v-if="store.error"
      class="px-4 py-3 rounded-2xl text-sm border"
      style="background: rgba(194, 123, 102, 0.08); border-color: rgba(194, 123, 102, 0.3); color: #C27B66;"
    >
      {{ store.error }}
    </div>

    <div
      v-if="actionError"
      class="px-4 py-3 rounded-2xl text-sm border flex justify-between gap-4 whitespace-pre-wrap"
      style="background: rgba(194, 123, 102, 0.08); border-color: rgba(194, 123, 102, 0.3); color: #C27B66;"
    >
      <span>{{ actionError }}</span>
      <button class="shrink-0 font-medium hover:opacity-70" @click="actionError = null">✕</button>
    </div>

    <div
      v-if="actionSuccess"
      class="px-4 py-3 rounded-2xl text-sm border"
      style="background: rgba(45, 58, 49, 0.06); border-color: rgba(45, 58, 49, 0.15); color: #2D3A31;"
    >
      {{ actionSuccess }}
    </div>

    <!-- Loading -->
    <div v-if="store.loading" class="flex flex-col gap-2">
      <div v-for="i in 4" :key="i" class="h-12 rounded-2xl bg-background-mute animate-pulse" />
    </div>

    <div v-else-if="store.rules.length === 0" class="text-text-muted text-sm py-6 text-center">
      尚無規則資料
    </div>

    <!-- Table -->
    <div v-else class="overflow-x-auto rounded-3xl border border-border" style="box-shadow: 0 4px 6px -1px rgba(45, 58, 49, 0.05);">
      <table class="w-full text-sm">
        <thead>
          <tr class="bg-background-soft border-b border-border">
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">系統名稱</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">序號</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">系統</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">規則說明</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">填寫人</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">建立日期</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="filteredRules.length === 0">
            <td colspan="7" class="px-5 py-6 text-center text-text-muted text-sm">無符合條件的規則</td>
          </tr>
          <template v-for="rule in filteredRules" :key="rule.id">
            <tr
              class="border-b border-border hover:bg-background-soft/60 transition-colors duration-200 cursor-pointer"
              @click="toggleExpand(rule.id)"
            >
              <td class="px-5 py-3.5 text-text font-medium">{{ rule.systemName }}</td>
              <td class="px-5 py-3.5 text-text-muted text-xs font-mono">{{ rule.serialNumber }}</td>
              <td class="px-5 py-3.5 text-text">{{ rule.system }}</td>
              <td class="px-5 py-3.5 text-text max-w-xs">
                <div class="line-clamp-2 leading-relaxed whitespace-pre-wrap">{{ rule.ruleDescription }}</div>
              </td>
              <td class="px-5 py-3.5 text-text">{{ rule.recorder }}</td>
              <td class="px-5 py-3.5 text-text-muted text-xs">{{ formatDate(rule.createdAt) }}</td>
              <td class="px-5 py-3.5">
                <div class="flex gap-2">
                  <button
                    class="px-3 py-1 rounded-full text-xs text-accent border border-accent/30
                           hover:bg-accent/8 transition-all duration-300"
                    @click.stop="openEdit(rule)"
                  >
                    編輯
                  </button>
                  <button
                    class="px-3 py-1 rounded-full text-xs text-terracotta border border-terracotta/30
                           hover:bg-terracotta/8 transition-all duration-300"
                    @click.stop="handleDelete(rule)"
                  >
                    刪除
                  </button>
                </div>
              </td>
            </tr>
            <tr
              v-if="expandedIds.has(rule.id)"
              class="border-b border-border"
              style="background: rgba(45, 58, 49, 0.03);"
            >
              <td colspan="7" class="px-6 py-4 text-sm text-text leading-relaxed whitespace-pre-wrap">
                {{ rule.ruleDescription }}
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <RuleFormModal
      v-model="showModal"
      :rule="editingRule"
      @saved="handleSaved"
    />
  </div>
</template>
