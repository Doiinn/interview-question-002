<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import { useRouter } from 'vue-router'
import { reactive } from 'vue'

// @ts-ignore
import { registerUser } from '@/api/backendService'

const router = useRouter()

const schema = z.object({
  user: z
    .string('Please fill User')
    .regex(/^\S+$/, { message: "User cannot contain spaces" })
    .min(6, 'Must be 6-20 characters long')
    .max(20, 'Must be 6-20 characters long'),
  password: z
    .string('Please fill password')
    .min(8, 'Must be 8-20 characters long')
    .max(20, 'Must be 8-20 characters long')
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

async function onSubmit(event: FormSubmitEvent<Schema>) {
  // console.log(event.data)

  try {
    const response = await registerUser({
      username: event.data.user,
      password: event.data.password
    })

    if (response.code === 201) {
      //test show modal success
      alert('success')
      router.push('/')
    } else {
      alert(response.message)
    }
  } catch (error) {
    alert('error')
    console.log(error)
  }
}
</script>

<template>
  <div class="flex justify-center w-full py-10">
    <UContainer>
      <UPageHeader title="Register" class="space-y-4"
        :ui="{ root: 'border-none', container: 'flex flex-col items-center justify-center text-center w-full', title: 'text-center text-primary' }" />
      <UForm :schema="schema" :state="state" class="mx-auto w-64 max-w-md space-y-4" @submit="onSubmit">
        <UFormField label="User" name="user">
          <UInput class="w-full" v-model="state.user" maxlength="20" />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput class="w-full" v-model="state.password" type="password" maxlength="20" />
        </UFormField>

        <UFormField label="Confirm Password" name="confirmPassword">
          <UInput class="w-full" v-model="state.confirmPassword" type="password" maxlength="20" />
        </UFormField>

        <UButton size="xl" class="w-full justify-center my-2" type="submit">
          Register
        </UButton>
      </UForm>
    </UContainer>
  </div>
</template>

<style></style>
