import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../client"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"

export const AddButton = ({ onSubmitted, ...props }) => {
  const deferFn = () =>
    client.post("/api/role/menus").then(({ menus = [] }) => {
      return resetIsDisplay(false, menus)
    })

  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <Async deferFn={deferFn}>
      {(async) => {
        return (
          <>
            <Modal.Disclosure
              use={Button}
              ref={disclosureRef}
              onClick={async.run}
              css={css`
                flex: unset;
              `}
              {...dialog}
            >
              {add()}
            </Modal.Disclosure>
            <Async.Pending>
              <LoadingBackdrop />
            </Async.Pending>
            <Async.Rejected>
              {(error) => (
                <AlertDialog {...error} finalFocusRef={disclosureRef} />
              )}
            </Async.Rejected>
            <Async.Fulfilled>
              {(menus) => {
                return (
                  dialog.visible && (
                    <SubmitDialog
                      role={{
                        name: "",
                        menus
                      }}
                      onSubmitted={() => {
                        dialog.hide()
                        onSubmitted && onSubmitted()
                      }}
                      dialog={dialog}
                      disclosureRef={disclosureRef}
                    />
                  )
                )
              }}
            </Async.Fulfilled>
          </>
        )
      }}
    </Async>
  )
}

export const EditButton = ({ id, onSubmitted, ...props }) => {
  const deferFn = () =>
    client.get(`/api/role/${id}`).then(({ role = {} }) => role)

  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <Async deferFn={deferFn}>
      {(fetchRole) => (
        <>
          <Modal.Disclosure
            ref={disclosureRef}
            use={Button}
            onClick={fetchRole.run}
            css={css`
              flex: unset;
            `}
            {...dialog}
          >
            {edit()}
          </Modal.Disclosure>
          <Async.Pending>
            <LoadingBackdrop />
          </Async.Pending>
          <Async.Rejected>
            {(error) => (
              <AlertDialog {...error} finalFocusRef={disclosureRef} />
            )}
          </Async.Rejected>
          <Async.Fulfilled>
            {(role) =>
              dialog.visible && (
                <SubmitDialog
                  role={role}
                  onSubmitted={() => {
                    dialog.hide()
                    onSubmitted && onSubmitted()
                  }}
                  dialog={dialog}
                  finalFocusRef={disclosureRef}
                />
              )
            }
          </Async.Fulfilled>
        </>
      )}
    </Async>
  )
}

const SubmitDialog = ({
  role = {},
  onClickCancel,
  onSubmitted,
  dialog,
  finalFocusRef,
  children,
  ...props
}) => {
  let { id } = role
  let [menus = [], setMenus] = useState(role.menus)

  const create = (data) =>
    client.post(`/api/role`, {
      data
    })

  const update = (data) =>
    client.put(`/api/role/${id}`, {
      data: {
        ...role,
        ...data
      }
    })

  const deferFn = ([data]) => {
    return id ? update(data) : create(data)
  }

  return (
    <Async deferFn={deferFn}>
      {(submit) => (
        <>
          <Formik
            initialValues={{ name: role.name }}
            onSubmit={(form) => {
              let data = {
                ...form,
                menus
              }

              submit.run(data)
            }}
          >
            <FormDialog
              menus={menus}
              setMenus={setMenus}
              dialog={dialog}
              finalFocusRef={finalFocusRef}
            />
          </Formik>
          <Async.Rejected>
            {(error) => <AlertDialog {...error} />}
          </Async.Rejected>
          <Async.Fulfilled>
            {(_) => {
              useEffect(() => {
                dialog.hide()
                onSubmitted && onSubmitted()
              }, [dialog.visible])

              return null
            }}
          </Async.Fulfilled>
        </>
      )}
    </Async>
  )
}

const FormDialog = ({ menus, setMenus, dialog, finalFocusRef, ...props }) => {
  const { values, submitForm, resetForm, isPending } = useFormikContext()
  let nameInputRef = useRef()

  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked
    setMenus(menus)
  }

  useEffect(() => {
    if (dialog.visible) {
      nameInputRef?.current?.focus()
    }
  }, [dialog.visible])

  return (
    <Dialog.Modal
      title={title()}
      showActionButtons
      actionButtonsProps={{
        isLoading: isPending,
        onClickSubmit: submitForm,
        onClickCancel: resetForm
      }}
      hideOnClickOutside={false}
      unstable_finalFocusRef={finalFocusRef}
      {...dialog}
    >
      <Form>
        <div
          css={css`
            display: grid;
            grid-template-rows: auto;
            grid-template-areas:
              "name input"
              "group table";
          `}
        >
          <Box
            css={css`
              grid-area: name;
            `}
            margin="major-1"
          >
            <Label
              htmlFor="name"
              marginTop="major-1"
              css={css`
                line-height: unset;
              `}
            >
              {name()}
            </Label>
          </Box>

          <Box
            css={css`
              grid-area: input;
            `}
            margin="major-1"
          >
            <Field
              name="name"
              validate={(value) => {
                if (!value) {
                  return "required"
                }
              }}
            >
              {({ field, form, meta }) => {
                let state = meta.error && "danger"
                return (
                  <FieldWrapper state={state}>
                    <Input id="name" inputRef={nameInputRef} {...field} />
                  </FieldWrapper>
                )
              }}
            </Field>
          </Box>
          <Box
            css={css`
              grid-area: group;
            `}
            margin="major-1"
          >
            <Label
              htmlFor="name"
              marginTop="major-1"
              css={css`
                line-height: unset;
              `}
            >
              {control()}
            </Label>
          </Box>
          <Box
            css={css`
              grid-area: table;
            `}
            margin="major-1"
          >
            <MenuTable menus={menus} setChecked={setChecked} />
          </Box>
        </div>
      </Form>
    </Dialog.Modal>
  )
}

const MenuTable = ({ menus = [], setChecked } = {}) => {
  return (
    <Table>
      {menus.map((menu, index) => {
        return (
          <MenuTableContent key={index} menu={menu} setChecked={setChecked} />
        )
      })}
    </Table>
  )
}

const MenuTableContent = ({ menu = {}, setChecked }) => {
  let { code: moduleCode, subMenus = [] } = menu
  let label = menuLabel([moduleCode])
  if (label === "") {
    return null
  }

  return (
    <>
      <Table.Head>
        <Table.Row>
          <Table.HeadCell colSpan="2">
            <MenuCheckbox menu={menu} setChecked={setChecked} />
          </Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {subMenus.map((menu) => {
          let { code: subMenuCode, subMenus: subMenuItems = [] } = menu
          return (
            <Table.Row key={subMenuCode}>
              <Table.Cell>
                <MenuCheckbox
                  path={[moduleCode]}
                  menu={menu}
                  setChecked={setChecked}
                />
              </Table.Cell>
              <Table.Cell>
                <div>
                  {subMenuItems.map((subMenuItem, index) => {
                    return (
                      <MenuCheckbox
                        key={index}
                        path={[moduleCode, subMenuCode]}
                        menu={subMenuItem}
                        setChecked={setChecked}
                      />
                    )
                  })}
                </div>
              </Table.Cell>
            </Table.Row>
          )
        })}
      </Table.Body>
    </>
  )
}

const MenuCheckbox = ({ path = [], menu = {}, setChecked }) => {
  let [checked, set] = useState(menu.isDisplay)

  let { code } = menu
  let menuPath = [...path, code]
  let label = menuLabel(menuPath)
  if (!label) {
    return null
  }

  return (
    <Checkbox
      label={label}
      checked={checked}
      onChange={(event) => {
        let isChecked = event.target.checked
        set(isChecked)
        setChecked(menuPath, isChecked)
      }}
      css={css`
        white-space: nowrap;
      `}
    ></Checkbox>
  )
}

const resetIsDisplay = (isDisplay, menus = []) => {
  return menus.map((menu) => {
    menu.isDisplay = isDisplay
    if (menu.subMenus) {
      menu.subMenus = resetIsDisplay(isDisplay, menu.subMenus)
    }
    return menu
  })
}

//TODO: i18n
const add = () => "新增"

const edit = () => "修改"

const title = () => "新增权限组"

const name = () => "权限组名称"

const control = () => "权限管理"
