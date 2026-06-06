<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'

const route = useRoute()

const routeLabels: Record<string, string> = {
  home: '首頁',
  about: '關於',
  '知識整理': '知識整理',
  '語意搜尋': '語意搜尋',
  '自訂詞典': '自訂詞典',
  'AI語意搜尋': 'AI 語意搜尋',
  '操作紀錄': '操作紀錄',
}

const pageTitle = computed(() => {
  const name = route.name ? String(route.name) : ''
  return routeLabels[name] ?? name
})

const grainStyle = {
  backgroundImage: `url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E")`,
  backgroundRepeat: 'repeat',
}
</script>

<template>
  <!-- Paper grain texture overlay -->
  <div
    class="pointer-events-none fixed inset-0 z-50 opacity-[0.015]"
    :style="grainStyle"
  />

  <div class="flex min-h-screen">
    <!-- Sidebar -->
    <aside
      class="w-[220px] shrink-0 sticky top-0 h-screen overflow-y-auto flex flex-col px-4 py-6
             bg-background border-r border-border"
    >
      <!-- Logo -->
      <div class="flex items-center gap-2.5 px-2 mb-10">
        <div class="w-7 h-7 rounded-full bg-accent flex items-center justify-center shrink-0">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
               fill="none" stroke="#F9F8F4" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
            <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z"/>
            <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12"/>
          </svg>
        </div>
        <span
          class="text-[0.95rem] font-semibold text-heading"
          style="font-family: 'Playfair Display', Georgia, serif; font-style: italic; letter-spacing: 0.01em;"
        >
          RAG 知識庫
        </span>
      </div>

      <nav class="flex flex-col gap-0.5">
        <RouterLink
          to="/"
          class="block px-3 py-2 rounded-full text-[0.875rem] text-text-muted no-underline
                 transition-all duration-300 hover:bg-background-soft hover:text-text
                 [&.router-link-exact-active]:bg-accent-muted [&.router-link-exact-active]:text-accent
                 [&.router-link-exact-active]:font-medium"
        >
          語意搜尋
        </RouterLink>
        <RouterLink
          to="/rules"
          class="block px-3 py-2 rounded-full text-[0.875rem] text-text-muted no-underline
                 transition-all duration-300 hover:bg-background-soft hover:text-text
                 [&.router-link-exact-active]:bg-accent-muted [&.router-link-exact-active]:text-accent
                 [&.router-link-exact-active]:font-medium"
        >
          知識庫
        </RouterLink>
        <RouterLink
          to="/jieba-dict"
          class="block px-3 py-2 rounded-full text-[0.875rem] text-text-muted no-underline
                 transition-all duration-300 hover:bg-background-soft hover:text-text
                 [&.router-link-exact-active]:bg-accent-muted [&.router-link-exact-active]:text-accent
                 [&.router-link-exact-active]:font-medium"
        >
          自訂詞典
        </RouterLink>
        <RouterLink
          to="/ai-search"
          class="block px-3 py-2 rounded-full text-[0.875rem] text-text-muted no-underline
                 transition-all duration-300 hover:bg-background-soft hover:text-text
                 [&.router-link-exact-active]:bg-accent-muted [&.router-link-exact-active]:text-accent
                 [&.router-link-exact-active]:font-medium"
        >
          AI 語意搜尋
        </RouterLink>
      </nav>

      <!-- Bottom decoration -->
      <div class="mt-auto pt-6 px-2">
        <div class="h-px bg-border" />
        <p class="mt-4 text-[0.75rem] text-text-muted leading-relaxed">
          語意搜尋 · 向量檢索<br>Powered by RAG
        </p>
      </div>
    </aside>

    <!-- Right column -->
    <div class="flex-1 flex flex-col min-h-screen">
      <!-- Topbar -->
      <header
        class="h-14 shrink-0 sticky top-0 z-10 flex items-center justify-between px-6
               bg-background border-b border-border"
        style="box-shadow: 0 1px 3px rgba(45, 58, 49, 0.06);"
      >
        <span
          class="text-[0.95rem] font-semibold text-heading"
          style="font-family: 'Playfair Display', Georgia, serif;"
        >
          {{ pageTitle }}
        </span>
        <div class="flex items-center gap-3">
          <span
            class="w-8 h-8 rounded-full bg-accent text-[0.8rem] font-semibold
                   flex items-center justify-center cursor-pointer"
            style="color: #F9F8F4;"
          >
            U
          </span>
        </div>
      </header>

      <!-- Main content -->
      <main class="flex-1 p-8 overflow-y-auto">
        <RouterView />
      </main>
    </div>
  </div>
</template>
