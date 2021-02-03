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
  Textarea as _Textarea,
  TextareaField as _TextareaField
} from "bumbag"

export const Textarea = ({
  name,
  register,
  registerOptions = {},
  textareaRef,
  resize = "none",
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (name && !textareaRef && register) {
    textareaRef = register({
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
    <_Textarea
      name={name}
      textareaRef={textareaRef}
      state={state}
      {...props}
      css={css`
        & textarea {
          resize: ${resize};
        }
      `}
    />
  )
}

export const TextareaField = ({
  name,
  register,
  registerOptions = {},
  textareaRef,
  resize = "none",
  validationText,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (name && !textareaRef && register) {
    textareaRef = register({
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
    <_TextareaField
      name={name}
      textareaRef={textareaRef}
      state={state}
      validationText={validationText}
      css={css`
        & textarea {
          resize: ${resize};
        }
      `}
      {...props}
    />
  )
}

export default Textarea
