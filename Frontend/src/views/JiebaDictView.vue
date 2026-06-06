<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useJiebaDictStore } from '@/stores/jiebaDict'
import type { JiebaDictPayload } from '@/api/jiebaDict'

const store = useJiebaDictStore()
onMounted(store.fetchAll)

const POS_OPTIONS = ['n', 'v', 'a', 'eng', 'x']

const blank = (): JiebaDictPayload => ({ word: '', frequency: 100, partOfSpeech: 'n', isActive: true })
const form = ref<JiebaDictPayload>(blank())
const editId = ref<number | null>(null)
const formError = ref('')
const saving = ref(false)

function startEdit(entry: { id: number } & JiebaDictPayload) {
  editId.value = entry.id
  form.value = { word: entry.word, frequency: entry.frequency, partOfSpeech: entry.partOfSpeech, isActive: entry.isActive }
  formError.value = ''
}

function cancelEdit() {
  editId.value = null
  form.value = blank()
  formError.value = ''
}

async function submit() {
  if (!form.value.word.trim()) { formError.value = '詞彙不可為空'; return }
  saving.value = true
  formError.value = ''
  try {
    if (editId.value !== null) {
      await store.update(editId.value, form.value)
      cancelEdit()
    } else {
      await store.create(form.value)
      form.value = blank()
    }
  } catch (e) {
    formError.value = (e as Error).message
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('確定刪除此詞彙？')) return
  try {
    await store.remove(id)
  } catch (e) {
    store.error = (e as Error).message
  }
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
          自訂詞典維護
        </h1>
        <p class="text-sm text-text-muted">管理 Jieba 分詞自訂詞彙</p>
      </div>
      <button
        :disabled="store.rebuilding"
        class="px-5 py-2.5 rounded-full bg-accent text-white text-sm font-medium
               hover:opacity-85 disabled:opacity-50 transition-all duration-300"
        @click="store.rebuild()"
      >
        {{ store.rebuilding ? '套用中…' : '套用詞典 & 重建索引' }}
      </button>
    </div>

    <!-- Alerts -->
    <div
      v-if="store.rebuildMessage"
      class="px-4 py-3 rounded-2xl text-sm border"
      style="background: rgba(45, 58, 49, 0.06); border-color: rgba(45, 58, 49, 0.15); color: #2D3A31;"
    >
      {{ store.rebuildMessage }}
    </div>
    <div
      v-if="store.error"
      class="px-4 py-3 rounded-2xl text-sm border"
      style="background: rgba(194, 123, 102, 0.08); border-color: rgba(194, 123, 102, 0.3); color: #C27B66;"
    >
      {{ store.error }}
    </div>

    <!-- Add / edit form -->
    <div
      class="rounded-3xl border border-border bg-background-soft p-6 flex flex-col gap-4"
      style="box-shadow: 0 4px 6px -1px rgba(45, 58, 49, 0.05);"
    >
      <p
        class="text-[0.95rem] font-semibold text-heading"
        style="font-family: 'Playfair Display', Georgia, serif;"
      >
        {{ editId !== null ? '編輯詞彙' : '新增詞彙' }}
      </p>
      <div class="flex gap-3 flex-wrap items-center">
        <input
          v-model="form.word"
          placeholder="詞彙（必填）"
          class="px-4 py-2.5 rounded-full border border-border bg-background text-text text-sm
                 focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300 w-40"
        />
        <input
          v-model.number="form.frequency"
          type="number"
          min="1"
          placeholder="詞頻"
          class="px-4 py-2.5 rounded-full border border-border bg-background text-text text-sm
                 focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300 w-24"
        />
        <select
          v-model="form.partOfSpeech"
          class="px-4 py-2.5 rounded-full border border-border bg-background text-text text-sm
                 focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300"
        >
          <option v-for="pos in POS_OPTIONS" :key="pos" :value="pos">{{ pos }}</option>
        </select>
        <label class="flex items-center gap-1.5 text-sm text-text cursor-pointer select-none">
          <input v-model="form.isActive" type="checkbox" class="rounded accent-accent" />
          啟用
        </label>
        <button
          :disabled="saving"
          class="px-5 py-2.5 rounded-full bg-accent text-white text-sm font-medium
                 hover:opacity-85 disabled:opacity-50 transition-all duration-300"
          @click="submit"
        >
          {{ saving ? '儲存中…' : (editId !== null ? '更新' : '新增') }}
        </button>
        <button
          v-if="editId !== null"
          class="px-5 py-2.5 rounded-full border border-border text-text-muted text-sm
                 hover:bg-background-mute transition-all duration-300"
          @click="cancelEdit"
        >
          取消
        </button>
      </div>
      <p v-if="formError" class="text-xs text-terracotta pl-1">{{ formError }}</p>
    </div>

    <!-- Skeletons -->
    <div v-if="store.loading" class="flex flex-col gap-2">
      <div v-for="i in 5" :key="i" class="h-10 rounded-2xl bg-background-mute animate-pulse" />
    </div>

    <div v-else-if="store.entries.length === 0" class="text-sm text-text-muted py-6 text-center">
      尚無自訂詞彙
    </div>

    <!-- Table -->
    <div
      v-else
      class="overflow-x-auto rounded-3xl border border-border"
      style="box-shadow: 0 4px 6px -1px rgba(45, 58, 49, 0.05);"
    >
      <table class="w-full text-sm">
        <thead>
          <tr class="bg-background-soft border-b border-border">
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">詞彙</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">詞頻</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">詞性</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">狀態</th>
            <th class="px-5 py-3.5 text-left text-xs text-text-muted font-medium tracking-wide uppercase">新增時間</th>
            <th class="px-5 py-3.5" />
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="entry in store.entries"
            :key="entry.id"
            class="border-b border-border last:border-0 hover:bg-background-soft/60 transition-colors duration-200"
          >
            <td class="px-5 py-3.5 font-medium text-text">{{ entry.word }}</td>
            <td class="px-5 py-3.5 text-text-muted">{{ entry.frequency }}</td>
            <td class="px-5 py-3.5">
              <span class="px-2 py-0.5 rounded-full bg-background-mute text-text-muted text-xs font-mono">
                {{ entry.partOfSpeech }}
              </span>
            </td>
            <td class="px-5 py-3.5">
              <span
                :class="entry.isActive
                  ? 'bg-accent/10 text-accent'
                  : 'bg-background-mute text-text-muted'"
                class="text-xs px-2.5 py-0.5 rounded-full font-medium"
              >
                {{ entry.isActive ? '啟用' : '停用' }}
              </span>
            </td>
            <td class="px-5 py-3.5 text-text-muted text-xs">
              {{ new Date(entry.createdAt).toLocaleDateString('zh-TW') }}
            </td>
            <td class="px-5 py-3.5">
              <div class="flex gap-2 justify-end">
                <button
                  class="text-xs px-3 py-1 rounded-full border border-accent/30 text-accent
                         hover:bg-accent/8 transition-all duration-300"
                  @click="startEdit(entry)"
                >
                  編輯
                </button>
                <button
                  class="text-xs px-3 py-1 rounded-full border border-terracotta/30 text-terracotta
                         hover:bg-terracotta/8 transition-all duration-300"
                  @click="remove(entry.id)"
                >
                  刪除
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
