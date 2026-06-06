<script setup lang="ts">
import AppPage from '@/components/AppPage.vue'
import AppCard from '@/components/AppCard.vue'
</script>

<template>
  <AppPage>
    <template #title>About</template>

    <AppCard>
      <p class="text-text">This is an about page</p>
    </AppCard>
  </AppPage>
</template>
