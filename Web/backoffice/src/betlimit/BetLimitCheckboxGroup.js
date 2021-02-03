import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Switch } from "bumbag"
import { useBetLimitList } from "./client"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllChecked, isAllPropChecked } from "../checkbox/isAllChecked"
import { Trans } from "../i18n"

export const BetLimitCheckboxGroup = ({
  name = "betLimitIds",
  arrayFieldName = "betLimits",
  width = "100%",
  disabled,
  ...props
}) => {
  let { control, setValue, watch } = useFormContext()
  let betLimits = watch(arrayFieldName)
  const { fields } = useFieldArray({
    control,
    name: arrayFieldName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllPropChecked(selection)
  return useMemo(() => {
    return (
      <Set orientation="vertical" width={width} {...props}>
        <Table width={width}>
          <Table.Head>
            <BetLimitCheckboxGroupTableHeadRow
              name={name}
              fields={fields}
              checkedFieldName={defaultCheckedFieldName}
              allChecked={allChecked}
              disabled={disabled}
            />
          </Table.Head>
          <Table.Body>
            {fields.map((betLimitField, index) => (
              <BetLimitCheckboxGroupTableRow
                key={betLimitField._id}
                name={name}
                disabled={disabled}
                field={betLimitField}
                index={index}
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

export const BetLimitCheckboxGroupTableHeadRow = ({
  name,
  fields,
  checkedFieldName = defaultCheckedFieldName,
  allChecked,
  onCheckedChange,
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
                let fieldName = checkedFieldName(name, field, index)
                setValue(fieldName, checked)
              })

              onCheckedChange?.(checked)
            }}
            disabled={disabled}
          />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="币种" en="Currency" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="最低投注" en="Min Bet" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="最高投注" en="Max Bet" />
        </Table.HeadCell>
        <Table.HeadCell textAlign="center">
          <Trans zh="单场最高投注" en="Max Bet Per Match" />
        </Table.HeadCell>
        {addonAfter}
      </Table.Row>
    )
  }, [JSON.stringify(fields), allChecked])
}

const defaultCheckedFieldName = (name, checkboxField) =>
  `${name}.${checkboxField.id}.checked`

export const BetLimitCheckboxGroupTableRow = ({
  name,
  disabled,
  field,
  index,
  checkedFieldName = defaultCheckedFieldName,
  onCheckedChange,
  addonAfter,
  ...props
}) => {
  let formContext = useFormContext()
  return useMemo(() => {
    let checkboxName = defaultCheckedFieldName(name, field, index)
    return (
      <Table.Row>
        <Table.Cell>
          <Checkbox
            key={field._id}
            name={checkboxName}
            checkboxRef={formContext.register()}
            defaultChecked={field.checked}
            disabled={disabled}
            onChange={(event) => {
              let checked = event.target.checked
              onCheckedChange?.(checked)
            }}
          />
        </Table.Cell>
        <Table.Cell textAlign="center">{field?.currencyCode}</Table.Cell>
        <Table.Cell textAlign="center">{field?.minBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBetPerMatch}</Table.Cell>
        {addonAfter}
      </Table.Row>
    )
  }, [field])
}
