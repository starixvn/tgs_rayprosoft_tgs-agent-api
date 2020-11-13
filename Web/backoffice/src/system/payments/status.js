import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select, Button } from "bumbag"
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

export const Label = ({ value }) => {
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

// Enabled <-> Disabled toggle
export const ToggleButton = ({ record, onSubmitted }) => {
  let { id, paymentStatus } = record
  let [newStatus, setSubmit] = useState()
  let { result, isLoading, error } = useChangePaymentStatus(id, newStatus)

  useEffect(() => {
    if (result && !error) {
      onSubmitted && onSubmitted()
    }
  }, [result, error])

  let palette =
    paymentStatus === DisabledName
      ? "danger"
      : paymentStatus === EnabledName
      ? "success"
      : "default"

  return (
    <Button
      isLoading={isLoading}
      palette={palette}
      onClick={() => {
        let newStatus =
          paymentStatus === DisabledName ? EnabledName : DisabledName
        setSubmit(newStatus)
      }}
      css={css`
        white-space: nowrap;
      `}
    >
      <Label value={record.paymentStatus} />
      {error && (
        <AlertDialog
          {...error}
          onClickClose={() => {
            setSubmit(undefined)
          }}
        />
      )}
    </Button>
  )
}
