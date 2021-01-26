import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text } from "bumbag"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { Trans } from "../i18n"
import { isAllChecked } from "../checkbox/isAllChecked"

export const CurrencyCheckboxGroupTable = ({
  name = "currencyCodes",
  fieldArrayName = "currencies",
  width = "100%",
  disabled = false,
  ...props
}) => {
  let { control, setValue, watch, register } = useFormContext()
  let currencies = watch(fieldArrayName)
  const { fields } = useFieldArray({
    control,
    name: fieldArrayName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllChecked(selection)

  return useMemo(() => {
    return (
      <Set orientation="vertical" width={width}>
        <Table width={width}>
          <Table.Head>
            <CurrencyCheckboxGroupTableHeadRow
              name={name}
              fields={fields}
              checkedFieldName={defaultCheckedFieldName}
              allChecked={allChecked}
              disabled={disabled}
            />
          </Table.Head>
          <Table.Body>
            {fields.map((field) => (
              <CurrencyCheckboxGroupTableRow
                key={field._id}
                name={name}
                disabled={disabled}
                field={field}
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

export const CurrencyCheckboxGroupTableHeadRow = ({
  name,
  fields,
  checkedFieldName = defaultCheckedFieldName,
  allChecked,
  disabled,
  addonAfter
}) => {
  let { control, setValue, watch, register } = useFormContext()

  return useMemo(() => {
    return (
      <Table.Row>
        <Table.HeadCell>
          <Checkbox
            checked={allChecked}
            onChange={(event) => {
              let checked = event.target.checked
              fields.forEach((field, index) => {
                setValue(checkedFieldName(name, field), checked)
              })
            }}
            disabled={disabled}
          />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="代码" en="Code" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="币种" en="Currency" />
        </Table.HeadCell>
        {addonAfter}
      </Table.Row>
    )
  }, [JSON.stringify(fields), allChecked])
}

export const CurrencyCheckboxGroupTableRow = ({
  name,
  disabled,
  field,
  checkedFieldName = defaultCheckedFieldName,
  addonAfter
}) => {
  let formContext = useFormContext()

  return useMemo(() => {
    return (
      <Table.Row>
        <Table.Cell textAlign="center">
          <Checkbox
            key={field._id}
            name={checkedFieldName(name, field)}
            checkboxRef={formContext?.register?.()}
            defaultChecked={field.checked}
            disabled={disabled}
          />
        </Table.Cell>
        <Table.Cell textAlign="center">{field.code}</Table.Cell>
        <Table.Cell textAlign="center">{field.name}</Table.Cell>
        {addonAfter}
      </Table.Row>
    )
  }, [field])
}

const defaultCheckedFieldName = (name, checkboxField) =>
  `${name}.${checkboxField.code}`
