import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { Input as _Input, InputField as _InputField } from "bumbag"
import { useTrans } from "../i18n"

export const HookFormInputField = ({
  name,
  register,
  registerOptions = {},
  inputRef,
  validationText,
  errors,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (!inputRef && register) {
    inputRef = register({
      required: props.required,
      min: props.min,
      max: props.max,
      minLength: props.minLength,
      maxLength: props.maxLength,
      validate: props.validate,
      ...registerOptions
    })
  }

  let error = formContext?.errors[name]
  let state = error && "danger"

  return <_Input name={name} inputRef={inputRef} state={state} {...props} />
}

export const InputField = ({
  name,
  register,
  registerOptions = {},
  inputRef,
  validationText,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  let defaultRequiredMessage = useRequiredMessage()

  if (!inputRef && register) {
    inputRef = register({
      required: {
        value: props.required,
        message: props.requiredMessage ?? defaultRequiredMessage
      },
      min: {
        value: props.min,
        message: props.minMessage
      },
      max: {
        value: props.max,
        message: props.maxMessage
      },
      minLength: {
        value: props.minLength,
        message: props.minLengthMessage
      },
      maxLength: {
        value: props.maxLength,
        message: props.maxLengthMessage
      },
      validate: props.validate,
      ...registerOptions
    })
  }

  let error = formContext?.errors[name]
  let state = error && "danger"
  validationText = validationText ?? error?.message

  return (
    <_InputField
      name={name}
      inputRef={inputRef}
      state={state}
      validationText={validationText}
      {...props}
    />
  )
}

const useRequiredMessage = () =>
  useTrans({
    zh: "必填栏",
    en: "Required field"
  })

export default HookFormInputField
