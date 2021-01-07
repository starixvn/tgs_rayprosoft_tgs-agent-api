import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { Spinner, Select as _Select } from "bumbag"

export const Select = ({
  name,
  register,
  registerOptions = {},
  selectRef,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register
  if (!selectRef && register) {
    selectRef = register({
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

  return <_Select name={name} selectRef={selectRef} state={state} {...props} />
}

export default Select
