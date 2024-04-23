<script setup>
import Board from '@/components/BoardComp.vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const gameId = route.params.gameId
</script>

<template>
  <main>
    <Board :game-id="gameId" />
  </main>
</template>

<style scoped>

</style>