<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'

import { reactive } from 'vue'

const schema = z.object({
  user: z
    .string('Please fill User')
    .regex(/^\S+$/, { message: "User cannot contain spaces" })
    .min(6, 'Must be at least 6 characters'),
  password: z
    .string('Please fill password').min(8, 'Must be at least 8 characters')
    .regex(/^\S+$/, { message: "Password cannot contain spaces" }),
  confirmPassword: z.string(),
})
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"]
  })

type Schema = z.output<typeof schema>

const state = reactive<Partial<Schema>>({
  user: '',
  password: undefined,
  confirmPassword: '',
})

// const toast = useToast()
async function onSubmit(event: FormSubmitEvent<Schema>) {
  // toast.add({ title: 'Success', description: 'The form has been submitted.', color: 'success' })
  console.log(event.data)
}
</script>

<template>
  <div class="flex justify-center w-full py-10">
    <UContainer>
      <UPageHeader title="Register" class="space-y-4" :ui="{ root: 'border-none', container: 'flex flex-col items-center justify-center text-center w-full', title: 'text-center text-primary' }" />
      <UForm :schema="schema" :state="state" class="mx-auto w-64 max-w-md space-y-4" @submit="onSubmit">
        <UFormField label="User" name="user">
          <UInput class="w-full" v-model="state.user" />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput class="w-full" v-model="state.password" type="password" />
        </UFormField>

        <UFormField label="Confirm Password" name="confirmPassword">
          <UInput class="w-full" v-model="state.confirmPassword" type="password" />
        </UFormField>

        <UButton size="xl" class="w-full justify-center my-2" type="submit">
          Register
        </UButton>
      </UForm>
    </UContainer>
  </div>
</template>

<style>

</style>
