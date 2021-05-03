import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Button,
    Set,
    Text,
    Input,
    Link,
    Dialog,
    Card,
    List,
    Level
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import {
    BrandPaymentStatusSelect,
    BrandPaymentStatusToggleButton
} from "./BrandPaymentStatus"
import { Type as TransferType } from "./transfer"
import { useChangeBrandPaymentOrders, useBrandPaymentList } from "./client"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import { Detail as ConditionDetail } from "./condition"
import { AddBrandPaymentButton, EditButton } from "./edit"
import { EditButton as FeeEditButton } from "./handlingfee"
import { ViewButton as LogsButton } from "./logs"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import Alert, { AlertDialog } from "../../dialog/Alert"
import { DesignatedPlayersLink } from "./DesignatedPlayersLink"
import { InputField } from "../../input/Input"
import { formatNumber } from "../../number/format"
import BrandBar from "../../report/BrandBar"
import textColumn from "../../table/column/textColumn"
import { PaymentProviderLabel } from "../../payment/PaymentProvider"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId,
        paymentStatus: null,
        keyword: ""
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let [transferType, setTransferType] = useState(TransferType.Deposit)

    let { isLoading, data, error, mutate, deleteCache } = useBrandPaymentList({
        ...toApiParams(state),
        transferType,
        sortby: "number"
    })

    let tab = useTabState({
        selectedId: transferType
    })

    const activeTab = (newTransferType) => () => {
        if (transferType === newTransferType) {
            mutate()
        }

        setTransferType(newTransferType)
        setState({
            pageIndex: 0,
            paymentStatus: null,
            keyword: ""
        })
    }
    let isDepositTab = transferType === TransferType.Deposit

    return (
        <PageContent
            title={<Trans zh="金流管理" en="Payment Management" />}
            actionBar={
                <Level>
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
                            setState({ pageIndex: 0, ...newState })
                        }}
                        isLoading={isLoading}
                    />
                    {isDepositTab && (
                        <AddBrandPaymentButton
                            brandId={brandId}
                            onSubmitted={mutate}
                        />
                    )}
                </Level>
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
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            transferType={transferType}
                            mutate={mutate}
                            isLoading={isLoading}
                            brandId={brandId}
                        />
                    )}
                </TabPanel>
                <TabPanel tabId={TransferType.Withdrawal} {...tab}>
                    {transferType === TransferType.Withdrawal && (
                        <DataTable
                            state={state}
                            data={data}
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            transferType={transferType}
                            mutate={mutate}
                            isLoading={isLoading}
                            brandId={brandId}
                        />
                    )}
                </TabPanel>
            </Tabs>
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

export const CodeContentPair = [Code, Content]

const DataTable = ({
    state,
    data,
    setPageIndex,
    setSortOrder,
    transferType,
    mutate,
    isLoading,
    brandId
}) => {
    let { pageSize } = state
    let [displayOrders, setDisplayOrder] = useState({})
    let [submitData, setSubmit] = useState()
    let {
        result,
        isLoading: isChangingDisplayOrder,
        error
    } = useChangeBrandPaymentOrders(submitData)

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
        items: data?.items ?? [],
        brandId
    })
    return (
        <Stack spacing="minor-1">
            <Table
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            <Dialog.Footer position="sticky" bottom="0">
                <Button
                    isLoading={isChangingDisplayOrder}
                    onClick={() => {
                        let list = Object.entries(displayOrders).map(
                            ([key, value]) => ({
                                id: key,
                                displayOrder: value
                            })
                        )
                        setSubmit({ orders: list })
                    }}
                >
                    <Trans zh="确认排序" en="Confirm Order" />
                </Button>
            </Dialog.Footer>
        </Stack>
    )
}

const SearchBar = ({ search, isLoading, ...props }) => {
    let formMethods = useForm()
    let keywordPlaceholder = useSearchInputPlaceHolder()
    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <BrandPaymentStatusSelect name="paymentStatus" />
                    <InputField
                        name="keyword"
                        defaultValue=""
                        showEmptyFieldValueAddonAfter
                        placeholder={keywordPlaceholder}
                        autocomplete="off"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({
    transferType,
    changeDisplayOrder,
    mutate,
    brandId,
    items = []
}) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([UpdateMenu])
    let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
    let hasEditAuth = useIsDisplayMenuByRoute([UpdateMenu])

    return useMemo(() => {
        let columns = [
            textColumn({
                id: "displayOrder",
                accessor: (record) => record,
                Header: <Trans zh="序" en="Order" />,
                disableSortBy: true,
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
            }),
            textColumn({
                accessor: "paymentId",
                Header: <Trans zh="支付厂商" en="Payment Name" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentProviderLabel value={value} />
            }),
            textColumn({
                accessor: "paymentMethodId",
                Header: <Trans zh="支付方式" en="Payment Method" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentMethodLabel value={value} />
            }),
            textColumn({
                accessor: "paymentChannelName",
                Header: <Trans zh="支付通道" en="Payment Channel" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "channelName",
                Header: <Trans zh="通道名称" en="Channel Name" />,
                disableSortBy: true
            }),
            textColumn({
                id: "target",
                accessor: (record) => record,
                Header: <Trans zh="开放对象" en="Target" />,
                disableSortBy: true,
                Cell: ({ value }) => (
                    <ConditionDetail brandId={brandId} record={value} />
                )
            }),
            textColumn({
                id: "designatedUsers",
                accessor: (record) => record,
                Header: <Trans zh="特定用户" en="Designated Users" />,
                disableSortBy: true,
                Cell: ({ value }) => <DesignatedPlayersLink record={value} />
            }),
            textColumn({
                id: "limit",
                accessor: (record) => record,
                Header: <Trans zh="单笔限额" en="Limit" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let from = formatNumber(value.transferLimitFrom)
                    let to = formatNumber(value.transferLimitTo)
                    return (
                        <Text.Block whiteSpace="nowrap">
                            {`${from} - ${to}`}
                        </Text.Block>
                    )
                }
            }),
            textColumn({
                id: "paymentStatus",
                accessor: (record) => record,
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <BrandPaymentStatusToggleButton
                            record={value}
                            disabled={!hasUpdateAuth}
                            onSubmitted={mutate}
                        />
                    )
                }
            })
        ]

        if (hasViewAuth || hasEditAuth) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    Cell: ({ value }) => {
                        return (
                            <Set alignX="center">
                                <EditButton
                                    brandId={brandId}
                                    record={value}
                                    onSubmitted={mutate}
                                />
                                <FeeEditButton
                                    brandId={brandId}
                                    record={value}
                                    onSubmitted={mutate}
                                />
                                <LogsButton brandId={brandId} record={value} />
                            </Set>
                        )
                    }
                })
            )
        }
        return columns
    }, [transferType, JSON.stringify(items)])
}

const useSearchInputPlaceHolder = () =>
    useTrans({
        zh: "关键字搜索",
        en: "Search..."
    })
