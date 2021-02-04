import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Checkbox as _Checkbox, CheckboxField as _CheckboxField } from "bumbag"

export const Checkbox = ({
  name,
  checkboxRef,
  register,
  registerOptions = {},
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (name && !checkboxRef && register) {
    checkboxRef = register({
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

  return (
    <_Checkbox name={name} checkboxRef={checkboxRef} state={state} {...props} />
  )
}

export const CheckboxField = ({
  name,
  checkboxRef,
  validationText,
  register,
  registerOptions = {},
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (name && !checkboxRef && register) {
    checkboxRef = register({
      required: {
        value: props.required,
        message: props.requiredMessage
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
    <_CheckboxField
      name={name}
      checkboxRef={checkboxRef}
      state={state}
      validationText={validationText}
      {...props}
    />
  )
}

export default Checkbox
