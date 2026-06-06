import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: '語意搜尋',
      component: () => import('../views/ChatDialogView.vue'),
    },
    {
      path: '/rules',
      name: '知識庫',
      component: () => import('../views/RuleMaintenanceView.vue'),
    },
    {
      path: '/jieba-dict',
      name: '自訂詞典',
      component: () => import('../views/JiebaDictView.vue'),
    },
    {
      path: '/ai-search',
      name: 'AI語意搜尋',
      component: () => import('../views/AiSemanticSearchView.vue'),
    },
    {
      path: '/99',
      name: '操作紀錄',
      component: () => import('../views/UserActionLogView.vue'),
    },
    {
      path: '/98',
      name: 'IP 使用統計',
      component: () => import('../views/RateLimitView.vue'),
    },
  ],
})

export default router
