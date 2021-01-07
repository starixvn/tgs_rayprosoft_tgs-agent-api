import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { RadioGroup as _RadioGroup } from "bumbag"

export const RadioGroup = ({ name, ...props }) => {
  let { control } = useFormContext() ?? {}

  return (
    <Controller
      control={control}
      name={name}
      render={(
        { onChange, onBlur, value, name, ref },
        { invalid, isTouched, isDirty }
      ) => (
        <_RadioGroup
          onBlur={onBlur}
          onChange={(e) => onChange(e.target.value)}
          value={value}
          {...props}
        />
      )}
    />
  )
}
