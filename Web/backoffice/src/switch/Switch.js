import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Spinner, Switch as _Switch, SwitchField as _SwitchField } from "bumbag"

export const Switch = ({
  name,
  switchRef,
  register,
  registerOptions = {},
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (!switchRef && register) {
    switchRef = register({
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

  return <_Switch name={name} switchRef={switchRef} state={state} {...props} />
}

export const SwitchField = ({
  name,
  switchRef,
  validationText,
  register,
  registerOptions = {},
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (!switchRef && register) {
    switchRef = register({
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
    <_SwitchField
      name={name}
      switchRef={switchRef}
      state={state}
      validationText={validationText}
      {...props}
    />
  )
}

export default Switch
