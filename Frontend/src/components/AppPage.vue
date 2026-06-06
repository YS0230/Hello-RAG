<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between min-h-[36px]">
      <h1
        class="text-xl font-semibold text-heading"
        style="font-family: 'Playfair Display', Georgia, serif;"
      >
        <slot name="title" />
      </h1>
      <div v-if="$slots.actions" class="flex items-center gap-2">
        <slot name="actions" />
      </div>
    </div>
    <slot />
  </div>
</template>
