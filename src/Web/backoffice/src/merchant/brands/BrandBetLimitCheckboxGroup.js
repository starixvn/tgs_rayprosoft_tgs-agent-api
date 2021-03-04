import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, Icon, Switch, Checkbox, Box } from "bumbag"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import { Trans, useTrans } from "../../i18n"
import Input from "../../input/Input"
import {
  BetLimitCheckboxGroupTableHeadRow,
  BetLimitCheckboxGroupTableRow
} from "../../betlimit/BetLimitCheckboxGroup"
import get from "lodash-es/get"

export const BrandBetLimitCheckboxGroupTable = ({
  name = "betLimits",
  fieldArrayContext,
  requiredCurrencyCodes,
  width = "100%",
  disabled = false,
  ...props
}) => {
  let {
    control,
    setValue,
    watch,
    register,
    getValues,
    errors = {},
    clearErrors
  } = useFormContext()
  const { fields } = fieldArrayContext

  let selection = watch(name)
  let allChecked = isAllPropChecked(selection)
  let checkedFieldErrors = mapCheckedFieldErrors(errors[name] ?? [])

  useEffect(() => {
    checkedFieldErrors.forEach((error) => {
      let { message } = error
      let { checkboxFieldName, currencyCode } = JSON.parse(message)
      if (!requiredCurrencyCodes.includes(currencyCode)) {
        clearErrors(checkboxFieldName)
      }
    })
  }, [checkedFieldErrors])

  return useMemo(() => {
    let fieldIndexPairs = fields.map((field, index) => [field, index])
    return (
      <Set orientation="vertical" width={width}>
        <Table width={width}>
          <Table.Head>
            <BetLimitCheckboxGroupTableHeadRow
              name={name}
              fields={fields}
              checkedFieldName={checkedFieldName}
              allChecked={allChecked}
              onCheckedChange={(checked) => {
                if (checked) {
                  //auto set first currency default
                  applyFirstCurrencyPairs((field, index) => {
                    let fieldName = defaultFieldName(name, field, index)
                    setValue(fieldName, true)
                  }, fieldIndexPairs)
                } else {
                  //remove all default
                  fieldIndexPairs.forEach(([field, index]) => {
                    let fieldName = defaultFieldName(name, field, index)
                    setValue(fieldName, false)
                  })
                }
              }}
              disabled={disabled}
              addonAfter={
                <Table.HeadCell textAlign="center">
                  <Trans zh="预设" en="Default" />
                </Table.HeadCell>
              }
            />
          </Table.Head>
          <Table.Body>
            {fields.map((field, index) => {
              let checkboxFieldName = checkedFieldName(name, field, index)
              let checkboxError = get(errors, checkboxFieldName)
              let checkboxState = checkboxError && "warning"

              let isDefaultFieldName = defaultFieldName(name, field, index)
              return (
                <BetLimitCheckboxGroupTableRow
                  key={field._id}
                  name={name}
                  disabled={disabled}
                  field={field}
                  index={index}
                  checkedFieldName={checkedFieldName}
                  checkboxState={checkboxState}
                  validateCheckbox={(value) => {
                    if (
                      requiredCurrencyCodes?.length > 0 &&
                      requiredCurrencyCodes.includes(field.currencyCode)
                    ) {
                      let checked = anyChecked(
                        name,
                        field.currencyCode,
                        getValues,
                        fieldIndexPairs
                      )
                      if (checked) {
                        return true //pass
                      }
                      return JSON.stringify({
                        checkboxFieldName,
                        currencyCode: field.currencyCode
                      })
                    }
                    return true //pass
                  }}
                  onCheckedChange={(checked) => {
                    let sameCurrencyFieldPairs = filterPairsByCurrency(
                      field.currencyCode,
                      fieldIndexPairs,
                      {
                        excludedId: field.id
                      }
                    )

                    if (checked) {
                      //auto set isDefault if there's no default
                      let foundOtherDefault = findCheckedDefault(
                        name,
                        getValues,
                        sameCurrencyFieldPairs
                      )
                      if (!foundOtherDefault) {
                        setValue(isDefaultFieldName, true)
                      }
                    } else if (!checked) {
                      setValue(isDefaultFieldName, false)

                      let foundOtherDefault = findCheckedDefault(
                        name,
                        getValues,
                        sameCurrencyFieldPairs
                      )

                      let checkedNotDefaultFields = filterCheckedNotDefault(
                        name,
                        getValues,
                        sameCurrencyFieldPairs
                      )
                      if (
                        !foundOtherDefault &&
                        checkedNotDefaultFields.length == 1
                      ) {
                        //only auto set default when "1" record found
                        let [field, index] = checkedNotDefaultFields[0]
                        setValue(defaultFieldName(name, field, index), true)
                      }
                    }
                  }}
                  addonAfter={
                    <Table.Cell textAlign="center">
                      <Box>
                        <Switch
                          name={isDefaultFieldName}
                          disabled={disabled}
                          switchRef={register()}
                          defaultChecked={field.isDefault}
                          justifyContent="center"
                          onChange={(event) => {
                            let checked = event.target.checked

                            if (checked) {
                              let sameCurrencyFieldPairs = filterPairsByCurrency(
                                field.currencyCode,
                                fieldIndexPairs,
                                {
                                  excludedId: field.id
                                }
                              )

                              sameCurrencyFieldPairs.forEach(
                                ([field, index]) => {
                                  let toggleChecked = !checked
                                  setValue(
                                    defaultFieldName(name, field, index),
                                    toggleChecked
                                  )
                                }
                              )
                            }
                          }}
                        />
                      </Box>
                    </Table.Cell>
                  }
                />
              )
            })}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [
    JSON.stringify(selection),
    JSON.stringify(fields),
    JSON.stringify(requiredCurrencyCodes ?? []),
    JSON.stringify(checkedFieldErrors)
  ])
}

const mapCheckedFieldErrors = (errors = []) => {
  let fieldName = "checked"
  return errors.map((error) => {
    let { type, message, ...context } = error[fieldName]
    return {
      type,
      message
    }
  })
}

const applyFirstCurrencyPairs = (fn, pairs = []) => {
  // currency code -> true | false
  let applied = {}

  pairs.reduce((applied, [field, index]) => {
    let { currencyCode } = field
    if (!applied[currencyCode]) {
      fn(field, index)
      applied[currencyCode] = true
    }

    return applied
  }, applied)
}

const filterPairsByCurrency = (code = "", pairs = [], options) => {
  return pairs.filter(([{ id, currencyCode }, index]) => {
    let excludedId = options?.excludedId
    if (excludedId && excludedId == id) {
      return false
    }

    return currencyCode === code
  })
}

const anyChecked = (name, currencyCode, getValues, fieldIndexPairs = []) => {
  return !!fieldIndexPairs
    .filter(([field, index]) => field.currencyCode === currencyCode)
    .find(([field, index]) => {
      let checked = getValues(checkedFieldName(name, field, index))
      return checked
    })
}

const filterCheckedNotDefault = (name, getValues, fieldIndexPairs = []) => {
  return fieldIndexPairs.filter(([field, index]) => {
    let checked = getValues(checkedFieldName(name, field, index))
    let isDefault = getValues(defaultFieldName(name, field, index))
    return checked && !isDefault
  })
}

const findCheckedDefault = (name, getValues, fieldIndexPairs = []) => {
  return fieldIndexPairs.find(([field, index]) => {
    let checked = getValues(checkedFieldName(name, field, index))
    let isDefault = getValues(defaultFieldName(name, field, index))
    return checked && isDefault
  })
}

const checkedFieldName = (name, field, index) => `${name}[${index}].checked`

const defaultFieldName = (name, field, index) => `${name}[${index}].isDefault`
