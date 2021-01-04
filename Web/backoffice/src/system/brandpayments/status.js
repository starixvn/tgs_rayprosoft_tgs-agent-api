import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Button } from "bumbag"
import { Trans, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { useChangeBrandPaymentStatus } from "./client"
import { HookFormSelect } from "../../select/HookFormSelect"
import { useFormContext, Controller } from "react-hook-form"

const Enabled = "Enabled"
const EnabledTrans = {
  zh: "开启",
  en: "Enabled"
}

const Disabled = "Disabled"
const DisabledTrans = {
  zh: "关闭",
  en: Disabled
}

export const Label = ({ value = "" }) => {
  switch (value) {
    case Disabled:
      return <Trans {...DisabledTrans} />

    case Enabled:
      return <Trans {...EnabledTrans} />

    default:
      return value
  }
}

export const BrandPaymentStatusSelect = ({ placeholder, ...props }) => {
  let defaultPlaceholder = useTrans({
    zh: "状态",
    en: "Status"
  })
  placeholder = placeholder ?? defaultPlaceholder

  let options = [
    {
      label: useTrans(EnabledTrans),
      value: Enabled
    },
    {
      label: useTrans(DisabledTrans),
      value: Disabled
    }
  ]

  return (
    <HookFormSelect placeholder={placeholder} options={options} {...props} />
  )
}

export const BrandPaymentStatusToggleButton = ({ name = "paymentStatus" }) => {
  let { watch, setValue, control } = useFormContext()
  let paymentStatus = watch(name)

  return (
    <Controller
      control={control}
      name={name}
      render={(
        { onChange, onBlur, value, name, ref },
        { invalid, isTouched, isDirty }
      ) => (
        <Button
          type="button"
          size="small"
          palette={toPalette(paymentStatus)}
          onClick={(event) => {
            let newStatus = toggleStatus(paymentStatus)
            setValue(name, newStatus)
          }}
        >
          <Label value={paymentStatus} />
        </Button>
      )}
    />
  )
}

const toggleStatus = (status) => {
  switch (status) {
    case Disabled:
      return Enabled
    case Enabled:
      return Disabled
    default:
      return status
  }
}

const toPalette = (status) => {
  switch (status) {
    case Disabled:
      return "danger"
    case Enabled:
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
  } = useChangeBrandPaymentStatus(id, newStatus)

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
      whiteSpace="nowrap"
    >
      <Label value={record.paymentStatus} />
      {error && <AlertDialog {...error} onClickClose={reset} />}
    </Button>
  )
}
