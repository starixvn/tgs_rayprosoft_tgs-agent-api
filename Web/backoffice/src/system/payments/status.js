import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select, Button } from "bumbag"
import { useFormikContext } from "formik"
import { Field as _Field } from "../../select/Field"
import { Trans, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { useChangePaymentStatus } from "./client"

const Enabled = 0
const EnabledName = "Enabled"
const EnabledTrans = {
  zh: "开启",
  en: "Enabled"
}

const Disabled = 1
const DisabledName = "Disabled"
const DisabledTrans = {
  zh: "关闭",
  en: DisabledName
}

//string -> number
export const from = (value) => {
  switch (value) {
    case DisabledName:
    case `${Disabled}`:
      return Disabled

    case EnabledName:
    case `${Enabled}`:
      return Enabled

    default:
      return value
  }
}

//number -> string
export const toString = (value) => {
  switch (value) {
    case Enabled:
      return EnabledName
    case Disabled:
      return DisabledName
    default:
      return typeof value === "string" ? value : ""
  }
}

export const Label = ({ value = "" }) => {
  switch (value) {
    case Disabled:
    case DisabledName:
      return <Trans {...DisabledTrans} />

    case Enabled:
    case EnabledName:
      return <Trans {...EnabledTrans} />

    default:
      return value
  }
}

export const Select = ({ name, required, ...props }) => {
  let enabled = {
    label: useTrans(EnabledTrans),
    value: EnabledName
  }

  let disabled = {
    label: useTrans(DisabledTrans),
    value: DisabledName
  }

  let options = required
    ? [enabled, disabled]
    : [
        {
          label: "",
          value: null
        },
        enabled,
        disabled
      ]
  return (
    <_Select name={name} required={required} options={options} {...props} />
  )
}

export const Field = ({ name, required, ...props }) => {
  return (
    <_Field name={name} required={required} {...props}>
      {(props) => <Select {...props} />}
    </_Field>
  )
}

export const ToggleButtonField = ({ name = "paymentStatus" }) => {
  let { values = {}, setFieldValue } = useFormikContext()
  let paymentStatus = Reflect.get(values, name)
  return (
    <Button
      type="button"
      size="small"
      palette={toPalette(paymentStatus)}
      onClick={(event) => {
        let newStatus = toggleStatus(paymentStatus)
        setFieldValue(name, newStatus)
      }}
    >
      <Label value={paymentStatus} />
    </Button>
  )
}

const toggleStatus = (status) => {
  switch (status) {
    case DisabledName:
      return EnabledName
    case EnabledName:
      return DisabledName
    default:
      return status
  }
}

const toPalette = (status) => {
  switch (status) {
    case DisabledName:
      return "danger"
    case EnabledName:
      return "success"
    default:
      return "default"
  }
}

// Enabled <-> Disabled toggle
export const ToggleButton = ({ record, onSubmitted, disabled, ...props }) => {
  let { id, paymentStatus } = record
  let [newStatus, setSubmit] = useState()
  let {
    result,
    isLoading,
    error,
    deleteCache,
    mutate
  } = useChangePaymentStatus(id, newStatus)

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      onSubmitted && onSubmitted()
    }
  }, [result, error])

  return (
    <Button
      isLoading={isLoading}
      disabled={isLoading || disabled}
      palette={toPalette(paymentStatus)}
      onClick={() => {
        reset()
        setSubmit(toggleStatus(paymentStatus))
      }}
      css={css`
        white-space: nowrap;
      `}
    >
      <Label value={record.paymentStatus} />
      {error && <AlertDialog {...error} onClickClose={reset} />}
    </Button>
  )
}
