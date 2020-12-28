import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Button,
  Text,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Table,
  Select,
  Group,
  Set,
  useClassName,
  InputField as _InputField
} from "bumbag"
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
  register = register ?? formContext.register

  let requiredErrorMsg = useRequiredMessage()
  if (!inputRef) {
    const validate = (value) => {
      if (
        props.required &&
        (value === undefined || value === null || value === "")
      ) {
        return requiredErrorMsg
      }

      return props?.validate?.(value)
    }

    inputRef = register({
      /*
        required: props.required,
        min: props.min,
        max: props.max,
        minLength: props.minLength,
        maxLength: props.maxLength,
        validate: props.validate
        */
      validate,
      ...registerOptions
    })
  }

  errors = errors ?? formContext.errors
  let error = errors[name]
  let state = error && "danger"
  let errorMessage = validationText ?? error?.message
  return (
    <_InputField
      name={name}
      inputRef={inputRef}
      state={state}
      validationText={errorMessage}
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
