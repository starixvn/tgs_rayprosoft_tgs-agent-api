import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, {
  useAsync,
  IfPending,
  IfRejected,
  IfFulfilled
} from "react-async"
import client from "../../client"
import { Formik, Field } from "formik"
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
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import {
  label as systemSubMenuLabel,
  Code as SystemCode,
  name as systemModuleLabel
} from "../index"
import { label as menuLabel, find as findMenu } from "../../menu"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const EditButton = ({ id, onSubmitted, ...props }) => {
  const deferFn = () =>
    client.get(`/api/role/${id}`).then(({ role = {} }) => role)

  let dialog = Modal.useState()
  let disclosureRef = useRef()

  return (
    <Async deferFn={deferFn}>
      {(async) => (
        <>
          <Modal.Disclosure
            ref={disclosureRef}
            use={Button}
            onClick={() => {
              async.run()
              //dialog.show()
            }}
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
              <AlertDialog {...error} disclosureRef={disclosureRef} />
            )}
          </Async.Rejected>
          <Async.Fulfilled>
            {(role) => (
              <>
                <EditDialog
                  role={role}
                  onSubmitted={() => {
                    console.log("onSubmitted")
                    //dialog.hide()
                    //TODO: refresh table
                  }}
                  dialog={dialog}
                  disclosureRef={disclosureRef}
                />
              </>
            )}
          </Async.Fulfilled>
        </>
      )}
    </Async>
  )
}

const EditDialog = ({
  role = {},
  onClickCancel,
  onSubmitted,
  dialog,
  disclosureRef,
  children,
  ...props
}) => {
  let { id } = role
  let [menus = [], setMenus] = useState(role.menus)

  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked
    setMenus(menus)
  }

  const deferFn = ([data]) => {
    return client
      .put(`/api/role/${id}`, {
        data: {
          ...role,
          ...data
        }
      })
      .then((response) => {
        onSubmitted && onSubmitted()
        return response
      })
  }

  return (
    <Async deferFn={deferFn}>
      {(asyncSubmit) => (
        <>
          <Formik
            initialValues={{ name: role.name }}
            onSubmit={(form) => {
              let data = {
                ...form,
                menus
              }

              asyncSubmit.run(data)
              console.log("submit", data)
            }}
          >
            {(formProps) => (
              <>
                <Dialog.Modal
                  title={title()}
                  showActionButtons
                  actionButtonsProps={{
                    isLoading: asyncSubmit.isPending,
                    onClickSubmit: formProps.submitForm,
                    onClickCancel: () => {
                      //setMenus([])
                      formProps.resetForm()
                    }
                  }}
                  hideOnClickOutside={false}
                  unstable_finalFocusRef={disclosureRef}
                  {...dialog}
                >
                  <form
                    onReset={formProps.handleReset}
                    onSubmit={formProps.handleSubmit}
                  >
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
                        <Field name="name">
                          {({ field, form, meta }) => (
                            <Input id="name" {...field} />
                          )}
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
                  </form>
                </Dialog.Modal>
              </>
            )}
          </Formik>
          <Async.Rejected>
            {({ title, errors }) => {
              const BumbagZIndex = 19900410
              let zIndex = `${BumbagZIndex + 100}`
              return (
                <AlertDialog
                  title={title}
                  errors={errors}
                  visible={true}
                  zIndex={zIndex}
                />
              )
            }}
          </Async.Rejected>
        </>
      )}
    </Async>
  )
}

export const AddButton = ({ onSubmitted, ...props }) => {
  let { data, run } = useAsync({
    deferFn: ([]) => {}
  })
  return <Modal.State></Modal.State>
}
/*
export const AddButton = ({ onSubmitted, ...props }) => {
  let { data, run: fetchMenus, ...asyncRoles } = useAsync({
    deferFn: getMenus
  })

  let [menus = [], setMenus] = useState(data)

  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked
    setMenus(menus)
  }

  //TODO: handle error
  let [promise, setSubmit] = useState(Promise.resolve())
  let submit = useAsync({ promise })

  useEffect(() => {
    console.log("shall reset?")
    setMenus(data)
  }, [data])

  //TODO: dialog loading status
  return (
    <Formik
      initialValues={{ name: "" }}
      onSubmit={(form) => {
        let data = {
          ...form,
          menus
        }

        let promise = client.post("/api/role", { data })
        promise.then((response) => {
          //TODO: need to check success?
          onSubmitted && onSubmitted()
          return response
        })

        setSubmit(promise)
      }}
    >
      {(formProps) => {
        return (
          <Modal.State>
            <Modal.Disclosure
              use={Button}
              onClick={fetchMenus}
              css={css`
                flex: unset;
              `}
            >
              {add()}
            </Modal.Disclosure>
            <Dialog.Modal
              title={title()}
              showActionButtons
              actionButtonsProps={{
                isLoading: submit.isPending,
                onClickSubmit: formProps.submitForm,
                onClickCancel: () => {
                  setMenus([])
                  formProps.resetForm()
                }
              }}
              hideOnClickOutside={false}
            >
              <form
                onReset={formProps.handleReset}
                onSubmit={formProps.handleSubmit}
              >
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
                    <Field name="name">
                      {({ field, form, meta }) => (
                        <Input id="name" {...field} />
                      )}
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
              </form>
            </Dialog.Modal>
          </Modal.State>
        )
      }}
    </Formik>
  )
}
*/

const MenuTable = ({ menus = [], setChecked }) => {
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

const getMenus = (id = "") => {
  return client.post("/api/role/menus").then(({ menus = [] }) => {
    return resetIsDisplay(false, menus)
  })
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
