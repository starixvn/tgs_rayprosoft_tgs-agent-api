import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Form, Field } from "formik"
import {
  Heading,
  Stack,
  Button,
  Set,
  InputField,
  Text,
  Box,
  Input,
  Link,
  Dialog,
  Card
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/Search"
import equals from "fast-deep-equal"
import {
  Field as StatusSelectField,
  Label as StatusLabel,
  from as toStatus,
  ToggleButton as StatusToggleButton
} from "./status"
import { Type as TransferType, from as transferTypeFrom } from "./transfer"
import { useChangePaymentOrders, usePaymentList } from "./client"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import {
  AddButton as AddVendorButton,
  AddMethodButton,
  AddChannelButton
} from "./vendor/edit"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { CloseText } from "../../button/ActionButtons"
import { Detail as ConditionDetail } from "./condition"
import { EditButton } from "./edit"
import { EditButton as FeeEditButton } from "./handlingfee"
import { ViewButton as LogsButton } from "./logs"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

const Body = () => {
  let [state, set] = useState({
    transferType: TransferType.Deposit,
    paymentStatus: null,
    keyword: ""
  })

  const setState = (state) => set((prevState) => ({ ...prevState, ...state }))

  let { transferType } = state
  let { data, isLoading, mutate } = usePaymentList({
    ...state,
    sortby: "number",
    showAll: true
  })

  let tab = useTabState({
    selectedId: transferType
  })

  const activeTab = (newTransferType) => () => {
    if (transferType === newTransferType) {
      mutate()
    }
    setState({
      transferType: newTransferType,
      paymentStatus: null,
      keyword: ""
    })
  }

  return (
    <PageContent
      title={<Trans zh="金流管理" en="Payment Management" />}
      actionBar={
        <>
          <SearchBar
            search={(newState) => {
              if (
                equals(newState, {
                  paymentStatus: state.paymentStatus,
                  keyword: state.keyword
                })
              ) {
                mutate()
              }
              setState(newState)
            }}
            isLoading={isLoading}
          />
          {/* <Set alignX="right" alignY="center">
            <AddVendorButton />
            <AddMethodButton />
            <AddChannelButton />
          </Set> */}
        </>
      }
    >
      <Tabs>
        <TabList>
          <Tab
            tabId={TransferType.Deposit}
            onClick={activeTab(TransferType.Deposit)}
            {...tab}
          >
            <Trans zh="存款" en="Deposit" />
          </Tab>
          <Tab
            tabId={TransferType.Withdrawal}
            onClick={activeTab(TransferType.Withdrawal)}
            {...tab}
          >
            <Trans zh="提款" en="Withdrawal" />
          </Tab>
        </TabList>
        <TabPanel tabId={TransferType.Deposit} {...tab}>
          {transferType === TransferType.Deposit && (
            <DataTable
              state={state}
              data={data}
              transferType={transferType}
              mutate={mutate}
              isLoading={isLoading}
            />
          )}
        </TabPanel>
        <TabPanel tabId={TransferType.Withdrawal} {...tab}>
          {transferType === TransferType.Withdrawal && (
            <DataTable
              state={state}
              data={data}
              transferType={transferType}
              mutate={mutate}
              isLoading={isLoading}
            />
          )}
        </TabPanel>
      </Tabs>
    </PageContent>
  )
}

export const CodeContentPair = [Code, Content]

const DataTable = ({ state, data, transferType, mutate, isLoading }) => {
  let [displayOrders, setDisplayOrder] = useState({})
  let [submitData, setSubmit] = useState()
  let {
    result,
    isLoading: isChangingDisplayOrder,
    error
  } = useChangePaymentOrders(submitData)

  const reset = () => {
    setDisplayOrder({})
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      mutate()
    }
  }, [result, error])

  const changeDisplayOrder = ({ id, value }) => {
    let newDisplayOrders = {
      ...displayOrders
    }
    Reflect.set(newDisplayOrders, id, value)
    setDisplayOrder(newDisplayOrders)
  }

  let columns = useColumns({
    transferType,
    mutate,
    changeDisplayOrder,
    items: data?.items ?? []
  })
  return (
    <Stack spacing="minor-1">
      <Table
        columns={columns}
        data={data?.items}
        isLoading={isLoading}
        usePagination={false}
      />
      <Button
        isLoading={isChangingDisplayOrder}
        onClick={() => {
          let list = Object.entries(displayOrders).map(([key, value]) => ({
            id: key,
            displayOrder: value
          }))
          setSubmit({ orders: list })
        }}
      >
        <Trans zh="确认排序" en="Confirm Order" />
      </Button>
    </Stack>
  )
}

const SearchBar = ({ search, isLoading, ...props }) => {
  return (
    <Formik
      initialValues={{
        paymentStatus: "",
        keyword: ""
      }}
      onSubmit={search}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <StatusSelectField name="paymentStatus" />
            <Field
              name="keyword"
              component={InputField.Formik}
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("keyword", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <SearchButton isLoading={isLoading} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const useColumns = ({
  transferType,
  changeDisplayOrder,
  mutate,
  items = []
}) => {
  let hasUpdateAuth = useIsDisplayMenuByRoute([UpdateMenu])
  let columns = [
    {
      id: "displayOrder",
      accessor: (record) => record,
      Header: <Trans zh="序" en="Order" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, number: displayOrder = "" } = value
        return (
          <Input
            type="number"
            size="small"
            min="0"
            minLength="1"
            width="5em"
            defaultValue={displayOrder}
            onChange={(event) => {
              let value = event.target.value
              let number = parseInt(value)
              if (Number.isFinite(number)) {
                changeDisplayOrder({
                  id,
                  value: number
                })
              }
            }}
          />
        )
      }
    },
    {
      accessor: "paymentVendorName",
      Header: <Trans zh="支付厂商" en="Payment Vendor" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "paymentMethodName",
      Header: <Trans zh="支付方式" en="Payment Method" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "paymentChannelName",
      Header: <Trans zh="支付通道" en="Payment Channel" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "channelName",
      Header: <Trans zh="通道名称" en="Channel Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      id: "target",
      accessor: (record) => record,
      Header: <Trans zh="开放对象" en="Target" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return <ConditionDetail record={value} />
      }
    },
    {
      id: "designatedUsers",
      accessor: (record) => record,
      Header: <Trans zh="特定用户" en="Designated Users" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        let { designatedUsers } = value
        if (!designatedUsers) {
          return <Trans zh="无" en="None" />
        }

        return <DesignatedUsersLink />
      }
    },
    {
      id: "limit",
      accessor: (record) => record,
      Header: <Trans zh="单笔限额" en="Limit" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        //TODO: format number?
        return (
          <Text.Block whiteSpace="nowrap">
            {value.transferLimitFrom} - {value.transferLimitTo}
          </Text.Block>
        )
      }
    },
    {
      id: "paymentStatus",
      accessor: (record) => record,
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <StatusToggleButton
            record={value}
            disabled={!hasUpdateAuth}
            onSubmitted={mutate}
          />
        )
      }
    }
  ]

  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let hasEditAuth = useIsDisplayMenuByRoute([UpdateMenu])
  if (hasViewAuth || hasEditAuth) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set alignX="center">
            <EditButton record={value} onSubmitted={mutate} />
            <FeeEditButton record={value} onSubmitted={mutate} />
            <LogsButton record={value} />
          </Set>
        )
      }
    })
  }
  return useMemo(() => columns, [transferType, JSON.stringify(items)])
}

const searchInputPlaceHolder = () =>
  useTrans({
    zh: "关键字搜索",
    en: "Search..."
  })

const DesignatedUsersLink = ({}) => {
  let dialog = useModalState()

  let title = useTrans({
    zh: "特定用户",
    en: "Designated Users"
  })
  //TODO: fetch users API
  let users = []
  return (
    <>
      <Disclosure use={Link} {...dialog}>
        <Trans zh="有" en="Users" />
      </Disclosure>
      {dialog.visible && (
        <Dialog.Modal
          title={title}
          footer={
            <Box alignX="right" width="100%">
              <Button onClick={dialog.hide}>
                <CloseText />
              </Button>
            </Box>
          }
          {...dialog}
        >
          <Card variant="bordered">
            <Set orientation="vertical" spacing="major-1">
              {users.map((user) => (
                <Text.Block>{user}</Text.Block>
              ))}
            </Set>
          </Card>
        </Dialog.Modal>
      )}
    </>
  )
}
